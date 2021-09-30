using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Json = Eu4ModViewer.Models.Json;

namespace Eu4ModViewer.Models.Output
{
	public class Country
	{
		public string Tag { get; set; }
		public string Name { get; set; }
		public IReadOnlyCollection<Json.Idea> Ideas { get; set; }
		public IReadOnlyCollection<Color> Colors { get; set; }
	}
}
