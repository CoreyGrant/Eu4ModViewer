using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Output
{
	public class ReligionGroup
	{
		public string Name { get; set; }
		public bool DefenderOfFaith { get; set; }
		public bool CanFormPersonalUnions { get; set; }
		public List<Religion> Religions { get; set; }
	}

	public class Religion
	{
		public string Name { get; set; }
		public Color Color { get; set; }
		public IReadOnlyDictionary<string, string> Country { get; set; }
		public IReadOnlyDictionary<string, string> SecondaryCountry { get; set; }
		public IReadOnlyDictionary<string, string> Province { get; set; }
		//public IReadOnlyCollection<ChurchAspect> Blessings { get; set; }
		//public IReadOnlyCollection<ChurchAspect> Aspects { get; set; }
	}
}
