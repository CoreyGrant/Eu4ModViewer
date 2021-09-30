using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eu4ModViewer.Shared.Api
{
	public interface ISteamApiFacade
	{
		Task<PublishedFileDetails> GetUGCFileDetails(long modId);
		Task<PublishedFileDetails> GetPublishedFileDetails(long modId);
	}

	public class SteamApiFacade : ISteamApiFacade
	{
		private readonly HttpClient _client;
		private readonly string steamApiKey;
		private const int appId = 236850;

		public SteamApiFacade(IConfiguration configuration)
		{
			_client = new HttpClient() { BaseAddress = new Uri("https://api.steampowered.com/") };
			steamApiKey = configuration["SteamApiKey"];
		}

		public async Task<PublishedFileDetails> GetUGCFileDetails(long ugcId)
		{
			var url = $"ISteamRemoteStorage/GetUGCFileDetails/v1/?key={steamApiKey}&ugcid={ugcId}&appid={appId}";
			var response = await _client.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
			}
			return new PublishedFileDetails();
		}

		public async Task<PublishedFileDetails> GetPublishedFileDetails(long modId)
		{
			if(modId == 0)
			{
				return new PublishedFileDetails {Description = "EU4 Base Game", Title = "Base Game", TimeUpdated = DateTime.Now.Ticks };
			}
			var url = $"ISteamRemoteStorage/GetPublishedFileDetails/v1/";
			var formParams = new Dictionary<string, string> { ["itemcount"] = "1", ["publishedfileids[0]"] = modId.ToString() };
			var formContent = new FormUrlEncodedContent(formParams);
			var response = await _client.PostAsync(url, formContent);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var details = JsonConvert.DeserializeObject<SteamApiResponse<PublishedFileDetailsResponse>>(content);
				return details.Response.PublishedFileDetails[0];
			}
			return null;
		}
	}

	public class SteamApiResponse<T>
	{
		[JsonProperty("response")]
		public T Response { get; set; }
	}

	public class PublishedFileDetailsResponse
	{
		[JsonProperty("result")]
		public int Result { get; set; }
		[JsonProperty("resultcount")]
		public int ResultCount { get; set; }
		[JsonProperty("publishedfiledetails")]
		public List<PublishedFileDetails> PublishedFileDetails { get; set; }
	}

	public class PublishedFileDetails
	{
		[JsonProperty("publishedfileid")]
		public long PublishedFileId { get; set; }
		[JsonProperty("title")]
		public string Title { get; set; }
		[JsonProperty("description")]
		public string Description { get; set; }
		[JsonProperty("time_updated")]
		public long TimeUpdated { get; set; }

		public DateTime LastUpdated => new DateTime(TimeUpdated);
		[JsonProperty("hcontent_file")]
		public long HContentFile { get; set; }
	}
}
