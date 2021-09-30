using Eu4ModViewer.Shared.Api;
using Eu4ModViewer.Shared.Database;
using Eu4ModViewer.Shared.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Shared
{
	public static class ServiceMappings
	{
		public static readonly ILoggerFactory loggerFactory
	= LoggerFactory.Create(builder => { });

		public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<Eu4ModViewerDatabase>(options => 
			{
				options.UseSqlServer(configuration.GetConnectionString("Eu4ModViewer"));
				options.UseLoggerFactory(loggerFactory);
			});
			services.AddTransient<ICloudFileService, CloudFileService>();
			services.AddTransient<IModService, ModService>();
			services.AddTransient<IModQueueService, ModQueueService>();
			services.AddTransient<ILogService, LogService>();
			services.AddSingleton<ISteamApiFacade, SteamApiFacade>();
			return services;
		}
	}
}
