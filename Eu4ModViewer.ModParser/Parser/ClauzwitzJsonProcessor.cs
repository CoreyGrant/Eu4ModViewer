using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Eu4ModViewer.Shared.Files;
using Eu4ModViewer.Shared.Entities;
using Eu4ModViewer.ModParser.Models;
using Eu4ModViewer.Models.Output;
using Json = Eu4ModViewer.Models.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eu4ModViewer.ModParser.Parser.Triggers;
using Eu4ModViewer.Models.Shared;
using Eu4ModViewer.Shared.Database;

namespace Eu4ModViewer.ModParser.Parser
{
	interface IClauzwitzJsonProcessor
	{
		long[] Process(List<Shared.Entities.ModQueue> mods);
	}

	class ClauzwitzJsonProcessor : IClauzwitzJsonProcessor
	{
		// C:\Program Files (x86)\Steam\steamapps\workshop\content\2368506
		private const string ModFolderPath = "C:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\236850\\";
		private const string BaseFolderPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Europa Universalis IV\\";

		private readonly ICloudFileService _cloudFileService;
		private readonly IClauzwitzToJsonFolderConverter _converter;
		private readonly IClauzwitzJsonLoader _jsonLoader;
		private readonly ILogService logService;

		public ClauzwitzJsonProcessor(
			ICloudFileService cloudFileService,
			IClauzwitzToJsonFolderConverter clauzwitzToJsonFolderConverter,
			IClauzwitzJsonLoader jsonLoader,
			ILogService logService)
		{
			_cloudFileService = cloudFileService;
			_converter = clauzwitzToJsonFolderConverter;
			_jsonLoader = jsonLoader;
			this.logService = logService;
		}

		public long[] Process(List<Shared.Entities.ModQueue> mods)
		{
			// Convert all mods and basegame
			var baseResult = _converter.Convert(BaseFolderPath, new string[0]);
			Console.WriteLine("Processed base game");
			var errorModIds = new List<long>();
			foreach (var mod in mods)
			{
				var excludeFiles = mod.ExcludeFiles?.Split("|") ?? new string[0];
				try
				{
					if (mod.ModId == 0)
					{
						ProcessMod(BaseFolderPath, 0, excludeFiles);
					}
					else
					{
						var path = Path.Combine(ModFolderPath, mod.ModId.ToString());
						ProcessMod(path, mod.ModId, excludeFiles, baseResult);
					}
					Console.WriteLine("Parsed mod " + mod.ModId);
				}
				catch(Exception ex)
				{
					logService.Log($"Failed to parse mod with id {mod.ModId}\nErrorMessage: {ex.Message}\nStackTrace: {ex.StackTrace}", Microsoft.Extensions.Logging.LogLevel.Warning);
					errorModIds.Add(mod.ModId);
				}
			}
			return errorModIds.ToArray();
		}

		private List<T> Merge<T>(List<T> one, List<T> two, Func<T, string> uniqueBy)
		{
			if (two == null)
			{
				return one;
			}
			var result = one.Concat(two).DistinctBy(uniqueBy).ToList();
			return result;
		}

		private ParseJsonResult ProcessMod(string gameDataPath, long modId, string[] excludeFiles, ParseJsonResult baseResult = null)
		{
			var result = _converter.Convert(gameDataPath, excludeFiles);
			var localizations = Merge(result.Localisations.ToList(), baseResult?.Localisations.ToList(), x => x.Key).ToDictionary(x => x.Key, x => x.Value);
			var countries = Merge(
				_jsonLoader.LoadCountries(result.CommonFolders["countries"]),
				_jsonLoader.LoadCountries(baseResult?.CommonFolders?["countries"]),
				x => x.Filename);

			var countryHistories = Merge(
				_jsonLoader.LoadCountryHistories(result.HistoryFolders?["countries"]),
				_jsonLoader.LoadCountryHistories(baseResult?.HistoryFolders?["countries"]),
				x => x.Filename);

			var countryTags = Merge(
				_jsonLoader.LoadCountryTags(result.CommonFolders["country_tags"]),
				_jsonLoader.LoadCountryTags(baseResult?.CommonFolders?["country_tags"]),
				x => x.Key);

			var ideas = Merge(
				_jsonLoader.LoadIdeas(result.CommonFolders["ideas"]),
				_jsonLoader.LoadIdeas(baseResult?.CommonFolders?["ideas"]),
				x => x.Name);

			var policies = Merge(
				_jsonLoader.LoadPolicies(result.CommonFolders["policies"]),
				_jsonLoader.LoadPolicies(baseResult?.CommonFolders?["policies"]),
				x => x.Name);

			var religions = Merge(
				_jsonLoader.LoadReligions(result.CommonFolders["religions"]),
				_jsonLoader.LoadReligions(baseResult?.CommonFolders?["religions"]),
				x => x.Name);

			var cultures = Merge(
				_jsonLoader.LoadCultures(result.CommonFolders["cultures"]),
				_jsonLoader.LoadCultures(baseResult?.CommonFolders?["cultures"]),
				x => x.Name);

			var countryIdeas = ideas.Where(x => string.IsNullOrEmpty(x.Category)).ToList();
			var ideaGroups = ideas.Where(x => !string.IsNullOrEmpty(x.Category)).ToList();

			// filter the country histories based on the country tags
			countryHistories = countryHistories.Where(x => countryTags.Any(ct => ct.Value.Split('/')[1].Trim() == x.Filename.Split('-')[1].Trim())).ToList();

			var religionsOutput = CombineReligionData(religions);
			var countriesOutput = CombineCountryData(countries, countryTags.ToDictionary(x => x.Key, x => x.Value), countryIdeas, countryHistories, religions, cultures);
			var ideaGroupsOutput = CombineIdeaGroups(ideaGroups, localizations);
			var policiesOutput = policies.Select(x => new Policy
			{
				Name = x.Name,
				Allow = x.Allow,
				Potential = x.Potential,
				Bonuses = x.Bonuses,
				MonarchPower = x.MonarchPower
			}).ToList();

			var religionBonuses = religionsOutput.SelectMany(x => x.Religions.SelectMany(y =>
				(y.Province ?? new Dictionary<string, string>()).Concat(y.SecondaryCountry ?? new Dictionary<string, string>()).Concat(y.Province ?? new Dictionary<string, string>()))
			).ToList();
			var ideaBonuses = ideas.SelectMany(x => x.Ideas.SelectMany(y => y.Bonuses)).ToList();
			var policyBonuses = policies.SelectMany(x => x.Bonuses).ToList();
			var bonuses = religionBonuses
				.Concat(ideaBonuses)
				.Concat(policyBonuses)
				.Select(x => x.Key)
				.Distinct().ToList();

			_cloudFileService.AddJsonFileAsync(new { bonuses }, "bonuses", modId);
			_cloudFileService.AddJsonFileAsync(new { religionGroups = religionsOutput }, "religionGroups", modId);
			_cloudFileService.AddJsonFileAsync(new { countries = countriesOutput }, "countries", modId);
			_cloudFileService.AddJsonFileAsync(new { ideaGroups = ideaGroupsOutput }, "ideaGroups", modId);
			_cloudFileService.AddJsonFileAsync(new { policies = policiesOutput }, "policies", modId);

			CreateCountryImages(countriesOutput, modId);
			return result;
		}
		private const char ParadoxReplacement = '§';
		private List<IdeaGroup> CombineIdeaGroups(List<Json.IdeaGroup> ideas, IReadOnlyDictionary<string, string> localizations)
		{
			var outputIdeas = new List<IdeaGroup>();
			foreach (var idea in ideas)
			{
				outputIdeas.Add(new IdeaGroup
				{
					Name = idea.Name,
					Trigger = idea.Trigger,
					Category = idea.Category,
					Ideas = idea.Ideas.Select(i => new Idea
					{
						Name = i.Name,
						Bonuses = i.Bonuses,
						LocalizedDesc = GetLocalization(i.Name + "_desc", localizations),
						LocalizedName = GetLocalization(i.Name, localizations)?.Split(ParadoxReplacement)?[0],
					}).ToList(),
					LocalizedDesc = GetLocalization(idea.Name + "_desc", localizations),
					LocalizedName = GetLocalization(idea.Name, localizations)?.Split(ParadoxReplacement)?[0],
				});
			}
			return outputIdeas;
		}

		private string GetLocalization(string key, IReadOnlyDictionary<string, string> loc)
		{
			var lowerKey = key.ToLower();
			return loc.ContainsKey(lowerKey)
				? loc[lowerKey]
				: null;
		}

		private List<ReligionGroup> CombineReligionData(
			List<Json.ReligionGroup> religionGroups/*,
			List<ChurchAspect> blessings*/)
		{
			return religionGroups.Select(group => new ReligionGroup
			{
				CanFormPersonalUnions = group.CanFormPersonalUnions == "yes",
				DefenderOfFaith = group.DefenderOfFaith == "yes",
				Name = group.Name,
				Religions = group.Religions.Select(religion => new Religion
				{
					Name = religion.Name,
					Color = new Color { Red = (int)Math.Round(decimal.Parse(religion.Color[0])), Green = (int)Math.Round(decimal.Parse(religion.Color[1])), Blue = (int)Math.Round(decimal.Parse(religion.Color[2])) },
					Country = religion.Country,
					SecondaryCountry = religion.CountryAsSecondary,
					Province = religion.Province,
					//Blessings = religion.Blessings
					//	?.Select(x => blessings.SingleOrDefault(y => y.Name == x))
					//	?.ToList(),
					//Aspects = religion.Aspects
					//	?.Select(x => blessings.SingleOrDefault(y => y.Name == x))
					//	?.ToList()
				}).ToList()
			}).ToList();
		}
		private string CountryNameMap(string name)
		{
			return name.ToLower() switch
			{
				"ukraine" => "Ukraine",
				"shu" => "Shun",
				"qic" => "QIC",
				"min" => "Min",
				"chw" => "Bachwezi",
				"chn" => "China",
				"afr" => "Toto",
				"peu" => "Peru",
				"lou" => "Louisiana",
				"cub" => "Cuba",
				"lap" => "La Plata",
				"vnz" => "Venezuela",
				"gzw" => "Great Zimbabwe",
				_ => Regex.Replace(name, "([a-z]{1})([A-Z]{1})", "$1 $2"),
			};
		}

		private List<Country> CombineCountryData(
			List<Json.Country> countries,
			IReadOnlyDictionary<string, string> countryTags,
			List<Json.IdeaGroup> countryIdeas,
			List<Json.CountryHistory> countryHistories,
			List<Json.ReligionGroup> religionGroups,
			List<Json.CultureGroup> cultureGroups)
		{
			var fullCountries = new List<Country>();
			var countryHistoryDict = countryHistories.ToDictionary(x => x.Filename.Split(new string[] { " - ", " -", "- ", "-" }, StringSplitOptions.RemoveEmptyEntries)[0], x => x);
			var cultureGroupsDict = cultureGroups.ToDictionary(x => x.Name, x => x);
			//var culturesDict = cultureGroups.SelectMany(x => x.Cultures).ToDictionary(x => x.Name, x => x);
			countryTags = countryTags.Where(x => x.Key != "REB" && x.Key != "PIR" && x.Key != "NAT" && x.Key != "SYN" && x.Key != "PAP").ToDictionary(x => x.Key, x => x.Value);
			foreach (var tag in countryTags)
			{
				if (!countryHistoryDict.ContainsKey(tag.Key))
				{
					continue;
				}
				var history = countryHistoryDict[tag.Key];
				// Should be Single but somehow failing
				var primaryCultureGroupName = cultureGroups.FirstOrDefault(x => x.Cultures.Any(y => y.Name == history.PrimaryCulture))?.Name;
				if (primaryCultureGroupName == null)
				{
					continue;
				}
				var religionGroupName = religionGroups.FirstOrDefault(x => x.Religions.Any(y => y.Name == history.Religion))?.Name;
				if (religionGroupName == null)
				{
					continue;
				}
				var query = new CountryIdeaQuery
				{
					Tag = tag.Key,
					CultureGroup = primaryCultureGroupName,
					PrimaryCulture = history.PrimaryCulture,
					Reforms = history.AddGovernmentReform,
					TechnologyGroup = history.TechnologyGroup,
					Religion = history.Religion,
					ReligionGroup = religionGroupName
				};
				var matchingIdeas = countryIdeas.Where(x => CountryIdeaTriggerResolver.Matches(x.Trigger, query)).ToList();
				var matchingIdea = MostSpecificIdeaGroup(matchingIdeas, query);
				if (matchingIdea == null) { continue; }
				var name = tag.Value.Replace("\"", "").Split("/").Last().Replace(".txt", "");
				var country = countries.SingleOrDefault(x => x.Filename.Replace(".txt", "") == name);
				var countryColor = country?.Color?.ToList();
				fullCountries.Add(new Country
				{
					Tag = tag.Key,
					Name = CountryNameMap(name),
					Ideas = matchingIdea.Ideas,
					Colors = country == null
						? new List<Color>()
						: new List<Color> { new Color { Red = int.Parse(countryColor[0]), Green = int.Parse(countryColor[1]), Blue = int.Parse(countryColor[2]) } }
				});
			}
			return fullCountries;
		}

		private Json.IdeaGroup MostSpecificIdeaGroup(List<Json.IdeaGroup> ideaGroups, CountryIdeaQuery query)
		{
			if (ideaGroups.Count == 0)
			{
				return null;
			}
			if (ideaGroups.Count == 1)
			{
				return ideaGroups.Single();
			}

			// Order of precidence (assuming)
			// Tag -> (Culture -> Culture Group)/(Religion -> Religion Group)/(Technology Group)/(Government Reform)

			// Alternatively, most important idea group might always be first, apart from default
			if (ideaGroups.Where(x => x.Name != "default_ideas").Count() >= 2)
			{
				return ideaGroups.First();
			}
			var cultureQuery = new CountryIdeaQuery
			{
				PrimaryCulture = query.PrimaryCulture
			};
			var cultureIdea = ideaGroups.Where(x => x.Trigger != null).SingleOrDefault(ig => CountryIdeaTriggerResolver.Matches(ig.Trigger, cultureQuery));
			if (cultureIdea != null)
			{
				return cultureIdea;
			}

			var cultureGroupQuery = new CountryIdeaQuery
			{
				CultureGroup = query.CultureGroup
			};
			var cultureGroupIdea = ideaGroups.Where(x => x.Trigger != null).SingleOrDefault(ig => CountryIdeaTriggerResolver.Matches(ig.Trigger, cultureGroupQuery));
			if (cultureGroupIdea != null)
			{
				return cultureGroupIdea;
			}

			var notDefault = ideaGroups.SingleOrDefault(ig => ig.Name != "default_ideas");
			if (notDefault != null)
			{
				return notDefault;
			}
			return ideaGroups.SingleOrDefault(ig => ig.Name == "default_ideas");
		}

		private readonly Dictionary<int, string> _ages = new Dictionary<int, string>
		{
			[0] = "age_of_discovery",
			[1] = "age_of_reformation",
			[2] = "age_of_absolutism",
			[3] = "age_of_revolutions",
		};

		private void CreateCountryImages(List<Country> countries, long modId)
		{
			var basePath = modId == 0
				? BaseFolderPath
				: Path.Combine(ModFolderPath, modId.ToString());
			var imageFolder = Path.Combine(basePath, "gfx", "flags");
			var imageFileNames = Directory.Exists(imageFolder)
				? Directory.GetFiles(imageFolder, "*")
				: new string[0];

			var baseFileNames = Directory.GetFiles(Path.Combine(BaseFolderPath, "gfx", "flags"));
			imageFileNames = imageFileNames.Concat(baseFileNames).ToArray();

			foreach (var country in countries)
			{
				var imageFileName = imageFileNames.FirstOrDefault(x => x.EndsWith($"{country.Tag}.tga"));
				if (imageFileName == null)
				{
					continue;
				}
				try
				{
					var stream = new MemoryStream();
					var iconBmp = TgaBmpConv.Load(imageFileName);
					iconBmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
					_cloudFileService.AddImageFileAsync(stream.ToArray(), $"{country.Tag}.png", modId, "flags");
				}
				catch (Exception ex) { }
			}
		}

		//private List<Policy> CollapsePoliciesByAge(List<Policy> policies)
		//{
		//	var policyAggregate = policies.Aggregate(new
		//	{
		//		dict = new Dictionary<string, Dictionary<string, List<Bonus>>>(),
		//		output = new List<Policy>()
		//	}, (prev, cur) =>
		//	{
		//		if (!string.IsNullOrEmpty(cur.Allow.CurrentAge))
		//		{
		//			if (!prev.dict.ContainsKey(cur.DisplayName))
		//			{
		//				prev.dict[cur.DisplayName] = new Dictionary<string, List<Bonus>>();
		//			}
		//			prev.dict[cur.DisplayName][cur.Allow.CurrentAge] = cur.Bonuses;
		//			if (prev.dict[cur.DisplayName].Count == 4)
		//			{
		//				var bonuses1 = prev.dict[cur.DisplayName][_ages[0]];
		//				var bonuses2 = prev.dict[cur.DisplayName][_ages[1]];
		//				var bonuses3 = prev.dict[cur.DisplayName][_ages[2]];
		//				var bonuses4 = prev.dict[cur.DisplayName][_ages[3]];
		//				prev.output.Add(new Policy
		//				{
		//					Name = cur.Name,
		//					Allow = cur.Allow,
		//					MonarchPower = cur.MonarchPower,
		//					Bonuses = bonuses1.Select(x => new Bonus
		//					{
		//						Type = x.Type,
		//						Value = x.Value + "/" + bonuses2.Single(y => y.Type == x.Type).Value + "/" + bonuses3.Single(y => y.Type == x.Type).Value + "/" + bonuses4.Single(y => y.Type == x.Type).Value
		//					}).ToList()
		//				});
		//				return prev;
		//			}
		//			else { return prev; }
		//		}
		//		prev.output.Add(cur);
		//		return prev;
		//	});
		//	return policyAggregate.output;
		//}
	}
}
