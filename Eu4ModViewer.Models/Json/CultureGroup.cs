using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class CultureGroup
	{
		public string Name { get; set; }
		public List<Culture> Cultures { get; set; }
	}

	public class Culture
	{
		public string Name { get; set; }
		public string Primary { get; set; }
	}
}
