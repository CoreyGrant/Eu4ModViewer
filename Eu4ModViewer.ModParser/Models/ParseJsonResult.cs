using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.ModParser.Models
{
	class ParseJsonResult
	{
		public IReadOnlyDictionary<string, string> Localisations { get; set; } = new Dictionary<string, string>();
		public Dictionary<string, Dictionary<string, JObject>> CommonFolders { get; set; } = new Dictionary<string, Dictionary<string, JObject>> 
		{
			["countries"] = null,
			["country_tags"] = null,
			["cultures"] = null,
			["religions"] = null,
			["ideas"] = null,
			["policies"] = null,
		};
		public Dictionary<string, JObject> CommonFiles { get; set; } = new Dictionary<string, JObject>();
		public Dictionary<string, Dictionary<string, JObject>> EventsFolders { get; set; } = new Dictionary<string, Dictionary<string, JObject>>();
		public Dictionary<string, JObject> EventsFiles { get; set; } = new Dictionary<string, JObject>();
		public Dictionary<string, Dictionary<string, JObject>> HistoryFolders { get; set; } = new Dictionary<string, Dictionary<string, JObject>> 
		{
			["countries"] = null
		};
		public Dictionary<string, JObject> HistoryFiles { get; set; } = new Dictionary<string, JObject>();
	}
}
