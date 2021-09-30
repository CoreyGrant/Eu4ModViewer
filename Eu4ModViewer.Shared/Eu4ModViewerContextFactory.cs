using Eu4ModViewer.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Shared
{
    public class Eu4ModViewerContextFactory : IDesignTimeDbContextFactory<Eu4ModViewerDatabase>
    {
		private readonly IConfiguration configuration;

		public Eu4ModViewerContextFactory(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

        public Eu4ModViewerDatabase CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Eu4ModViewerDatabase>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Eu4ModViewer"));

            return new Eu4ModViewerDatabase(optionsBuilder.Options);
        }
    }
}
