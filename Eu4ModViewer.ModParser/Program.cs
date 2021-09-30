using Eu4ModViewer.ModParser.Parser;
using Eu4ModViewer.Shared;
using Eu4ModViewer.Shared.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steamworks;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Eu4ModViewer.ModParser
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				using IHost host = CreateHostBuilder(args).Build();
				RunLoop(host).Wait();
			} catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}
		}
		static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hbc, services) =>
					services.AddShared(hbc.Configuration)
						.AddTransient<IQueueRunner, QueueRunner>()
						.AddTransient<IClauzwitzJsonProcessor, ClauzwitzJsonProcessor>()
						.AddTransient<IClauzwitzJsonLoader, ClauzwitzJsonLoader>()
						.AddTransient<IClauzwitzToJsonConverter, ClauzwitzToJsonConverter>()
						.AddTransient<IClauzwitzYamlLoader, ClauzwitzYamlLoader>()
						.AddTransient<IClauzwitzToJsonFolderConverter, ClauzwitzToJsonFolderConverter>()
				);
		private static async Task RunLoop(IHost host)
		{
			try
			{
				Steamworks.SteamClient.Init(236850, true);
			}
			catch (System.Exception e)
			{
				// Something went wrong! Steam is closed?
			}
			try
			{
				var period = 1000 * 60 * 5;
				var runner = (IQueueRunner)host.Services.GetService(typeof(IQueueRunner));
				var timer = new Stopwatch();
				while (true)
				{
					timer.Start();
					await runner.Run();

					var elapsed = (int)timer.ElapsedMilliseconds;
					timer.Reset();
					if(elapsed < period)
					{
						Thread.Sleep(period - elapsed);
					}
				}
			}
			catch (Exception ex)
			{
				var exceptionMessage = $"QueueRunner failed with Message: {ex.Message}\nStackTrace: {ex.StackTrace}";
				var logService = (ILogService)host.Services.GetService(typeof(ILogService));
				logService.Log(exceptionMessage, Microsoft.Extensions.Logging.LogLevel.Error);
				Console.WriteLine(exceptionMessage);
				Console.WriteLine("Restarting...");
				await RunLoop(host);
			}
		}
	}
}
