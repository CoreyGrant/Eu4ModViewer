using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class IdeaGroup
	{
		public string Name { get; set; }
		public string Category { get; set; }
		public List<Idea> Ideas { get; set; }

		public Trigger Trigger { get; set; }
	}

	public class Idea
	{
		public string Name { get; set; }
		public Dictionary<string, string> Bonuses { get; set; }
	}
}
