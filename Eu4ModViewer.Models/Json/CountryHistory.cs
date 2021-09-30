using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Json
{
	public class CountryHistory
	{
		[JsonProperty("_filename")]
		public string Filename { get; set; }
		[JsonProperty("government")]
		public string Government { get; set; }
		[JsonProperty("add_government_reform")]
		[JsonConverter(typeof(CustomArrayConverter<string>))]
		public List<string> AddGovernmentReform { get; set; }
		[JsonProperty("government_rank")]
		[JsonConverter(typeof(CustomArrayConverter<int>))]
		public List<int> GovernmentRank { get; set; }
		[JsonProperty("mercantilism")]
		public decimal Mercantilism { get; set; }
		[JsonProperty("technology_group")]
		[JsonConverter(typeof(CustomArrayConverter<string>))]
		public List<string> TechnologyGroup { get; set; }
		[JsonProperty("religion")]
		public string Religion { get; set; }
		[JsonProperty("primary_culture")]
		public string PrimaryCulture { get; set; }
		[JsonProperty("capital")]
		public int Capital { get; set; }
		[JsonProperty("fixed_capital")]
		public int FixedCapital { get; set; }
	}

	internal class CustomArrayConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(List<T>);
		}

		public override object ReadJson(
		  JsonReader reader,
		  Type objectType,
		  object existingValue,
		  JsonSerializer serializer)
		{
			JToken token = JToken.Load(reader);
			if (token.Type == JTokenType.Array)
				return token.ToObject<List<T>>();
			return new List<T> { token.ToObject<T>() };
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
