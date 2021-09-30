using Eu4ModViewer.ModParser.Parser;
using Eu4ModViewer.Shared.Api;
using Eu4ModViewer.Shared.Database;
using Eu4ModViewer.Shared.Entities;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eu4ModViewer.ModParser
{
	interface IQueueRunner
	{
		Task Run();
	}

	class QueueRunner : IQueueRunner
	{
		private readonly IModService _modService;
		private readonly IModQueueService _modQueueService;
		private readonly ILogService _logService;
		private readonly IClauzwitzJsonProcessor clauzwitzJsonProcessor;
		private readonly ISteamApiFacade steamApiFacade;

		public QueueRunner(
			IModService modService,
			IModQueueService modQueueService,
			ILogService logService,
			IClauzwitzJsonProcessor clauzwitzJsonProcessor,
			ISteamApiFacade steamApiFacade)
		{
			this._modService = modService;
			this._modQueueService = modQueueService;
			this._logService = logService;
			this.clauzwitzJsonProcessor = clauzwitzJsonProcessor;
			this.steamApiFacade = steamApiFacade;
		}

		public async Task Run()
		{
			var modQueues = _modQueueService.GetPending();
			var mods = new List<Mod>();
			var modQueuesToProcess = new List<ModQueue>();
			var existingIds = new List<long>();
			foreach (var modQueue in modQueues)
			{
				if (modQueue.ModId != 0)
				{
					var modExists = _modService.DoesModExist(modQueue.ModId);
					
					Console.WriteLine($"Picked up queue item with id {modQueue.ModQueueId} for mod {modQueue.ModId}");
					if (modExists)
					{
						existingIds.Add(modQueue.ModId);
					}
				
					await DownloadMod(modQueue.ModId);

					mods.Add(new Mod
					{
						ModId = modQueue.ModId,
						Description = modQueue.Description,
						LastUpdated = modQueue.LastUpdated,
						Name = modQueue.Title,
					});
				}
				else
				{
					mods.Add(new Mod { ModId = 0, Description = modQueue.Description, Name = modQueue.Title, LastUpdated = modQueue.LastUpdated });
				}
				modQueuesToProcess.Add(modQueue);
			}
			if (!mods.Any())
			{
				Console.WriteLine("No mods");
				return;
			}
			var errorIds = clauzwitzJsonProcessor.Process(modQueuesToProcess);
			var completeMods = mods.Where(x => !errorIds.Contains(x.ModId));
			foreach (var completeMod in completeMods)
			{
				if (existingIds.Contains(completeMod.ModId))
				{
					_modService.UpdateMod(completeMod);
				}
				else
				{
					_modService.CreateModAsync(completeMod);
				}
				_modQueueService.MarkAsSuccess(completeMod.ModId);
			}
			foreach (var errorModId in errorIds)
			{
				_modQueueService.MarkAsFailed(errorModId);
			}
			// Clean up the temp folders the function downloads to
			Console.WriteLine("Completed queue run");
		}
		private async Task DownloadMod(long modId)
		{
			await SteamUGC.DownloadAsync(new PublishedFileId { Value = (ulong)modId });
		}

		private async Task<PublishedFileDetails> GetModNameAndDesc(long modId)
		{
			try
			{
				var res = await this.steamApiFacade.GetPublishedFileDetails(modId);

				return res;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
	}
}
