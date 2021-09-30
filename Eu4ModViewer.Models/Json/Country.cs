using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class Country
	{
		[JsonProperty("_filename")]
		public string Filename { get; set; }
		[JsonProperty("color")]
		public IReadOnlyCollection<string> Color { get; set; }
	}
}
