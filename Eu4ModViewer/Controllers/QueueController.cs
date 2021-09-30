using Eu4ModViewer.Models.Queue;
using Eu4ModViewer.Shared.Api;
using Eu4ModViewer.Shared.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer.Controllers
{
	[ApiController]
	[Route("api/Queue")]
	public class QueueController : ControllerBase
	{
		private readonly IModQueueService modQueueService;
		private readonly IModService modService;
		private readonly ISteamApiFacade steamApiFacade;

		public QueueController(IModQueueService modQueueService, IModService modService, ISteamApiFacade steamApiFacade)
		{
			this.modQueueService = modQueueService;
			this.modService = modService;
			this.steamApiFacade = steamApiFacade;
		}

		[HttpPost("QueueMod")]
		public async Task<IActionResult> QueueMod(QueueModRequest request)
		{
			if(request.ModId < 0)
			{
				return this.BadRequest();
			}
			var modQueueId = modQueueService.AddToQueue(new Shared.Entities.ModQueue
			{
				ModId = request.ModId,
				Status = 0,
				Description = request.Description,
				Title = request.Title,
				ExcludeFiles = request.ExcludeFiles,
				LastUpdated = request.LastUpdated,
			});
			return this.Ok(new { result = new QueueModResult { ModQueueId = modQueueId, Success = true } });
		}

		[HttpGet("ModExists/{modId}")]
		public IActionResult ModExists(long modId)
		{
			return this.Ok(new { result = this.modService.DoesModExist(modId) });
		}

		[HttpGet("ModQueued/{modId}")]
		public IActionResult ModQueued(long modId)
		{
			return this.Ok(new { result = this.modQueueService.ModIsQueued(modId) });
		}

		[HttpGet("QueuedMods")]
		[ResponseCache(Duration = 30)]
		public IActionResult QueuedMods()
		{
			var latestMods = this.modQueueService.GetLatest(30);
			return this.Ok(new { result = latestMods });
		}

		[HttpPost("QueueStart")]
		public IActionResult QueueStart()
		{
			return this.Ok();
		}

		[HttpPost("QueueEnd")]
		public IActionResult QueueEnd()
		{
			return this.Ok();
		}
	}
}
