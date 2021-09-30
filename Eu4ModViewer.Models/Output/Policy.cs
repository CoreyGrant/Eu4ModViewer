using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Output
{
	public class Policy
	{
		public string Name { get; set; }
		public string MonarchPower { get; set; }
		public Trigger Potential { get; set; }
		public Trigger Allow { get; set; }
		public IReadOnlyDictionary<string, string> Bonuses { get; set; }
	}
}
