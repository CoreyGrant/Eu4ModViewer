using Eu4ModViewer.Models.Json;
using Eu4ModViewer.Models.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Eu4ModViewer.ModParser.Parser
{
	interface IClauzwitzJsonLoader
	{
		List<Country> LoadCountries(Dictionary<string, JObject> countryFiles);
		List<CountryHistory> LoadCountryHistories(Dictionary<string, JObject> countryHistoryFiles);
		List<KeyValuePair<string, string>> LoadCountryTags(Dictionary<string, JObject> countryTagFiles);
		List<CultureGroup> LoadCultures(Dictionary<string, JObject> cultureFiles);
		List<IdeaGroup> LoadIdeas(Dictionary<string, JObject> ideaFiles);
		List<Policy> LoadPolicies(Dictionary<string, JObject> ideaFiles);
		List<ReligionGroup> LoadReligions(Dictionary<string, JObject> religionFiles);
	}

	class ClauzwitzJsonLoader : IClauzwitzJsonLoader
	{
		public List<Country> LoadCountries(Dictionary<string, JObject> countryFiles)
		{
			var list = new List<Country>();
			if(countryFiles == null) { return list; }
			foreach (var countryFile in countryFiles)
			{
				var country = LoadObject<Country>(countryFile.Value);
				list.Add(country);
			}
			return list;
		}

		public List<CountryHistory> LoadCountryHistories(Dictionary<string, JObject> countryHistoryFiles)
		{
			var list = new List<CountryHistory>();
			if (countryHistoryFiles == null) { return list; }
			foreach (var file in countryHistoryFiles)
			{
				var countryHistory = LoadObject<CountryHistory>(file.Value);
				list.Add(countryHistory);
			}
			return list;
		}

		public List<KeyValuePair<string, string>> LoadCountryTags(Dictionary<string, JObject> countryTagFiles)
		{
			var dict = new Dictionary<string, string>();
			if (countryTagFiles == null) { return dict.ToList(); }
			foreach (var countryTagFile in countryTagFiles)
			{
				var props = countryTagFile.Value.Properties().Where(x => x.Name != "_filename");
				foreach (var tag in props)
				{
					dict[tag.Name] = tag.Value.ToString().Replace("\"", "");
				}
			}
			return dict.ToList();
		}

		public List<IdeaGroup> LoadIdeas(Dictionary<string, JObject> ideaFiles)
		{
			var ideaGroups = new List<IdeaGroup>();
			if (ideaFiles == null) { return ideaGroups; }
			foreach (var obj in ideaFiles)
			{
				var ideaGroupProps = obj.Value.Properties().Where(x => x.Name != "_filename");
				foreach (var ideaGroupProp in ideaGroupProps)
				{
					var ideaGroupObj = (JObject)ideaGroupProp.Value;
					var trigger = LoadTrigger((JObject)ideaGroupObj["trigger"]);
					var ideaProps = ideaGroupObj.Properties()
						.Where(x => x.Name != "trigger"
							&& x.Name != "category"
							&& x.Name != "free" && x.Name != "important");
					var ideaGroup = new IdeaGroup
					{
						Name = ideaGroupProp.Name,
						Category = ideaGroupObj.Value<string>("category"),
						Ideas = ideaProps.Select(x => new Idea
						{
							Name = x.Name,
							Bonuses = ((JObject)x.Value).Properties().ToDictionary(y => y.Name, y => y.Value is JArray jVal
							? jVal.Last().Value<string>()
							: y.Value.Value<string>())
						}).ToList(),
						Trigger = trigger,
					};
					ideaGroups.Add(ideaGroup);
				}
			}
			return ideaGroups;
		}

		//public List<GreatProject> LoadGreatProjects(string folder, string[] except = null)
		//{
		//	var fileNames = Directory.GetFiles(Path.Combine(folder, "common", "policies"));
		//	if (except != null)
		//	{
		//		fileNames = fileNames.Where(x => !except.Any(y => x.EndsWith(y + ".json"))).ToArray();
		//	}
		//	var greatProjects = new List<GreatProject>();
		//	foreach (var fileName in fileNames)
		//	{
		//		var obj = LoadObject(fileName);
		//		var props = obj.Properties().Where(x => x.Name != "_filename");
		//		foreach (var prop in props)
		//		{
		//			var greatProjectObj = (JObject)prop.Value;

		//		}
		//	}
		//}

		public List<Policy> LoadPolicies(Dictionary<string, JObject> ideaFiles)
		{
			var policies = new List<Policy>();
			if (ideaFiles == null) { return policies; }
			foreach (var obj in ideaFiles)
			{
				var props = obj.Value.Properties().Where(x => x.Name != "_filename");
				foreach (var prop in props)
				{
					var policyObj = (JObject)prop.Value;
					var bonusProps = policyObj.Properties().Where(x => x.Name != "monarch_power" && x.Name != "potential" && x.Name != "allow");

					var policy = new Policy
					{
						Name = prop.Name,
						MonarchPower = policyObj.Value<string>("monarch_power"),
						Potential = LoadTrigger((JObject)policyObj["potential"]),
						Allow = LoadTrigger((JObject)policyObj["allow"]),
						Bonuses = bonusProps.ToDictionary(x => x.Name, x => x.Value.Value<string>())
					};

					policies.Add(policy);
				}
			}
			return policies;
		}

		private readonly string[] ReligionGroupPropNameIgnore = new[] { "defender_of_faith", "can_form_personal_unions", "center_of_religion", "flags_with_emblem_percentage", "flag_emblem_index_range", "harmonized_modifier", "crusade_name", "religious_schools", "ai_will_propagate_through_trade" };

		public List<ReligionGroup> LoadReligions(Dictionary<string, JObject> religionFiles)
		{
			var religionGroups = new List<ReligionGroup>();
			if (religionFiles == null) { return religionGroups; }
			foreach (var obj in religionFiles)
			{
				var religionGroupProps = obj.Value.Properties().Where(x => x.Name != "_filename");
				foreach (var religionGroupProp in religionGroupProps)
				{
					var religionGroupValue = (JObject)religionGroupProp.Value;
					var religionProps = religionGroupValue.Properties()
						.Where(x => !ReligionGroupPropNameIgnore.Contains(x.Name));
					var religions = religionProps.Select(x =>
					{
						var religionValue = (JObject)x.Value;
						return new Religion
						{
							Name = x.Name,
							Color = ((JArray)religionValue["color"]).Select(x => x.Value<string>()).ToList(),
							Country = ((JObject)religionValue["country"]).Properties().ToDictionary(x => x.Name, x => x.Value.Value<string>()),
							CountryAsSecondary = ((JObject)religionValue["country_as_secondary"])?.Properties()?.ToDictionary(x => x.Name, x => x.Value.Value<string>()),
							Province = ((JObject)religionValue["province"])?.Properties()?.ToDictionary(x => x.Name, x => x.Value.Value<string>()),
							Icon = religionValue.Value<string>("icon")
						};
					}).ToList();
					var religionGroup = new ReligionGroup
					{
						Name = religionGroupProp.Name,
						DefenderOfFaith = religionGroupValue.Value<string>("defender_of_faith"),
						CanFormPersonalUnions = religionGroupValue.Value<string>("can_form_personal_unions"),
						CenterOfReligion = religionGroupValue.Value<string>("center_of_religion"),
						Religions = religions,
					};
					religionGroups.Add(religionGroup);
				}
			}
			return religionGroups;
		}

		public List<CultureGroup> LoadCultures(Dictionary<string, JObject> cultureFiles)
		{
			var cultureGroups = new List<CultureGroup>();
			if (cultureFiles == null) { return cultureGroups; }
			foreach (var obj in cultureFiles)
			{
				var cultureGroupProps = obj.Value.Properties().Where(x => x.Name != "_filename");
				foreach (var cultureGroupProp in cultureGroupProps)
				{
					var cultureProps = ((JObject)cultureGroupProp.Value).Properties().Where(x => x.Name != "graphical_culture" && x.Name != "second_graphical_culture");
					var cultureGroup = new CultureGroup
					{
						Name = cultureGroupProp.Name,
						Cultures = cultureProps.Select(x => new Culture
						{
							Name = x.Name,
							Primary = x.Value.Value<string>("primary"),
						}).ToList()
					};
					cultureGroups.Add(cultureGroup);
				}
			}
			return cultureGroups;
		}

		private T LoadObject<T>(JObject obj)
		{
			return obj.ToObject<T>();
		}

		private Trigger LoadTrigger(JObject triggerObj)
		{
			if (triggerObj == null)
			{
				return null;
			}
			var trigger = new Trigger
			{
				Conditions = new List<TriggerCondition>(),
				ConditionSets = new List<TriggerConditionSet>(),
			};
			var props = triggerObj.Properties();
			foreach (var prop in props)
			{
				var upperName = prop.Name.ToUpper();
				if (upperName == "OR" || upperName == "AND" || upperName == "NOT" || upperName == "HIDDEN_TRIGGER" || upperName == "CALC_TRUE_IF")
				{
					trigger.ConditionSets.Add(LoadTriggerConditionSet(prop.Value,
						prop.Name == "OR",
						prop.Name == "NOT"));
				}
				else
				{
					if (prop.Value is JArray arrayVal)
					{
						foreach (var val in arrayVal)
						{
							trigger.Conditions.Add(new TriggerCondition
							{
								Name = prop.Name,
								Value = val.Value<string>()
							});
						}
					}
					else
					{
						//TODO: Fix this properly, we need to take this into account
						if (prop.Name == "capital_scope") { continue; }
						trigger.Conditions.Add(new TriggerCondition
						{
							Name = prop.Name,
							Value = prop.Value.Value<string>(),
						});
					}
				}
			}
			return trigger;
		}

		private TriggerConditionSet LoadTriggerConditionSet(
			JToken csTok,
			bool composeOr,
			bool modifierNot)
		{
			var props = csTok is JObject csObj
				? csObj.Properties()
				: ((JArray)csTok).SelectMany(x => ((JObject)x).Properties());
			var cs = new TriggerConditionSet
			{
				ComposeOr = composeOr,
				ModifierNot = modifierNot,
				Conditions = new List<TriggerCondition>(),
				ConditionSets = new List<TriggerConditionSet>()
			};
			foreach (var prop in props)
			{
				var upperName = prop.Name.ToUpper();
				if (upperName == "OR" || upperName == "AND" || upperName == "NOT" || upperName == "HIDDEN_TRIGGER" || upperName == "CALC_TRUE_IF")
				{
					cs.ConditionSets.Add(LoadTriggerConditionSet(
						prop.Value,
						prop.Name == "OR",
						prop.Name == "NOT"));
				}
				else
				{
					if (prop.Value is JArray arrayVal)
					{
						foreach (var val in arrayVal)
						{
							cs.Conditions.Add(new TriggerCondition
							{
								Name = prop.Name,
								Value = val.Value<string>()
							});
						}
					}
					else
					{
						if (prop.Name == "capital_scope") { continue; }
						cs.Conditions.Add(new TriggerCondition
						{
							Name = prop.Name,
							Value = prop.Value.ToString(),
						});
					}
				}
			}
			return cs;
		}
	}
}
