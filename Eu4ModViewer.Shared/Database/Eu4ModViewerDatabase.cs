using Eu4ModViewer.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Shared.Database
{
	public class Eu4ModViewerDatabase : DbContext
	{
		public Eu4ModViewerDatabase(DbContextOptions<Eu4ModViewerDatabase> options): base(options) {
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Mod>().ToTable("Mod");
			modelBuilder.Entity<ModQueue>().ToTable("ModQueue");
			modelBuilder.Entity<Log>().ToTable("Log");
		}
		public DbSet<Mod> Mod { get; set; }
		public DbSet<ModQueue> ModQueue { get; set; }
		public DbSet<Log> Log { get; set; }
	}
}
