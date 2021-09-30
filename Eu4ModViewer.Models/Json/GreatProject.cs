using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class GreatProject
	{
		public string Name { get; set; }
		public string LocalizedName { get; set; }
		public string Date { get; set; }
		public int TimeInMonths { get; set; }
		public int BuildCost { get; set; }
	}
}
