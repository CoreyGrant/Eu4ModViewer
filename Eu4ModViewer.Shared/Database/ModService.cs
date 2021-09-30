using Eu4ModViewer.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4ModViewer.Shared.Database
{
	public interface IModService
	{
		void CreateModAsync(Mod mod);
		bool DoesModExist(long modId);
		List<Mod> GetMods();
		Mod GetMod(long modId);
		Mod UpdateMod(Mod mod);
	}

	public class ModService : IModService
	{
		private readonly Eu4ModViewerDatabase _database;

		public ModService(Eu4ModViewerDatabase database)
		{
			_database = database;
		}

		public void CreateModAsync(Mod mod)
		{
			_database.Mod.Add(mod);
			_database.SaveChanges();
		}

		public bool DoesModExist(long modId)
		{
			return _database.Mod.Any(x => x.ModId == modId);
		}

		public Mod GetMod(long modId)
		{
			return _database.Mod.Single(x => x.ModId == modId);
		}

		public List<Mod> GetMods()
		{
			return _database.Mod.ToList();
		}

		public Mod UpdateMod(Mod mod)
		{
			var rMod = _database.Mod.Update(mod).Entity;
			_database.SaveChanges();
			return rMod;
		}
	}
}
