using Eu4ModViewer.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSession();
			services.AddMvc();
			services.AddShared(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseSession();
			app.UseRouting();
			app.UseStaticFiles();
			var indexHtml = File.ReadAllText(Path.Combine(env.WebRootPath, "index.html"));
			app.Use(async (context, next) => {
				var request = context.Request;
				if (!request.Path.Value.Contains("/api/") && !request.Path.Value.Contains(".png"))
				{
					await context.Response.WriteAsync(indexHtml);
				}
				await next();
			});
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
