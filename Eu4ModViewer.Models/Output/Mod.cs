using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Output
{
	public class ModList
	{
		public const int BaseGameId = 0;
		public List<Mod> Mods { get; set; }
	}

	public class Mod
	{
		public string Name { get; set; }
		public long Id { get; set; }
		public List<ModSection> Sections { get; set; }
		public string Bonuses { get; set; }
	}

	public class ModSection
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public bool Unique { get; set; }
		public bool SomeBase { get; set; }
		public bool AllBase { get; set; }
	}
}
