using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class ReligionGroup
	{
		public string Name { get; set; }
		public string DefenderOfFaith { get; set; }
		public string CanFormPersonalUnions { get; set; }
		public string CenterOfReligion { get; set; }
		public List<Religion> Religions { get; set; }
	}

	public class Religion
	{
		public string Name { get; set; }
		public List<string> Color { get; set; }
		public string Icon { get; set; }
		public Dictionary<string, string> Country { get; set; }
		public Dictionary<string, string> CountryAsSecondary { get; set; }
		public Dictionary<string, string> Province { get; set; }
	}
}
