using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Eu4ModViewer.ModParser.Parser.ParserUtils;

namespace Eu4ModViewer.ModParser.Parser
{
	class JsonConverterIgnore
	{
		public string[] Global { get; set; }

		public bool Should(string value)
		{
			return Global.Contains(value);
		}
	}

	interface IClauzwitzToJsonConverter
	{
		JObject ParseObject(string filename, string fullFolderPath);
	}

	class ClauzwitzToJsonConverter : IClauzwitzToJsonConverter
	{
		private readonly JsonConverterIgnore _ignore;

		public ClauzwitzToJsonConverter()
		{
			_ignore = new JsonConverterIgnore
			{
				Global = new[] { "ai_will_do", "male_names", "female_names", "dynasty_names" },
			};
		}

		public JObject ParseObject(string filename, string fullFolderPath)
		{
			using (var sr = new StreamReader(File.OpenRead(Path.Combine(fullFolderPath, filename)), ParserUtils.Encoding.Windows1252))
			{
				return (JObject)ParseObject(sr, filename);
			}
		}

		private JToken ParseObject(StreamReader sr, string filename = null)
		{
			var obj = new JObject();
			var properties = new List<KeyValuePair<string, object>>();
			if (!string.IsNullOrEmpty(filename))
			{
				properties.Add(new KeyValuePair<string, object>("_filename", filename));
			}
			while (true)
			{
				var character = (char)sr.Read();
				// Get the stream to the start of something interesting
				character = SkipUntilNot(sr, character, NewLines.Concat(Spaces));
				// We have reached the end of the object, end the loop
				if (character == ObjectClose || character == Eof)
				{
					break;
				}
				string propName;
				// Read until we reach something which tells us the property name is done, or the object has closed
				(character, propName) = ReadUntil(sr, character, new[] { PropertySeperator, ObjectClose });
				if (propName.Trim() == "I03_surface_research_initiative")
				{
					var a = 2;
				}
				if (character == ObjectClose)
				{
					// We've reached the end of the object, without finding a property seperator
					// It's a value list
					var values = SplitList(propName);
					var jarray = JArray.FromObject(values);
					return jarray;
				}
				else if (character == PropertySeperator)
				{
					// It's a kv pair, read the value side, either as a value or another object
					character = SkipUntilNot(sr, character, NewLines.Concat(Spaces).Concat(new[] { PropertySeperator }));
					if (character == ObjectOpen)
					{
						var objValue = ParseObject(sr);
						properties.Add(new KeyValuePair<string, object>(propName.Trim(), objValue));
					}
					else
					{
						string propValue;
						(character, propValue) = ReadPropertyStringValue(sr, character);
						properties.Add(new KeyValuePair<string, object>(propName.Trim(), propValue.Trim()));
					}
				}
			}

			var groupedProperties = properties.GroupBy(x => x.Key);
			foreach (var groupedProp in groupedProperties)
			{
				if (_ignore.Should(groupedProp.Key))
				{
					continue;
				}

				if (groupedProp.Count() == 1)
				{
					obj[groupedProp.Key] = JToken.FromObject(groupedProp.Single().Value);
				}
				else
				{
					var values = groupedProp.Select(x => x.Value);
					obj[groupedProp.Key] = JArray.FromObject(values);
				}
			}
			return obj;
		}
		private List<string> SplitList(string list)
		{
			bool inQuotes = false;
			var buffer = "";
			var output = new List<string>();
			foreach (var character in list)
			{
				if (character == Quote)
				{
					inQuotes = !inQuotes;
					continue;
				}
				if (!inQuotes && NewLinesAndSpaces.Contains(character))
				{
					if (buffer == "" || buffer == "\r" || buffer == "\n") { buffer = ""; continue; }
					output.Add(buffer);
					buffer = "";
					continue;
				}
				buffer += character;
			}
			if (!string.IsNullOrEmpty(buffer)) { output.Add(buffer); };
			return output;
		}
	}


}
