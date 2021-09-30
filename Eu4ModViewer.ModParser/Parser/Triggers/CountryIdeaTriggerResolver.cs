using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eu4ModViewer.ModParser.Parser.Triggers
{
	static class CountryIdeaTriggerResolver
	{
		public static bool Matches(Trigger trigger, CountryIdeaQuery query)
		{
			// An ideaGroup without a trigger matches every country
			if (trigger == null) { return true; }
			// AND
			foreach (var c in trigger.Conditions)
			{
				if (!ResolveCondition(c, query))
				{
					return false;
				}
			}
			foreach (var cs in trigger.ConditionSets)
			{
				if (!ResolveConditionSet(cs, query))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ResolveCondition(TriggerCondition c, CountryIdeaQuery query)
		{
			if (c.Name == "always")
			{
				return c.Value == "yes";
			}
			if (c.Name == "tag")
			{
				return c.Value == query.Tag;
			}
			if (c.Name == "primary_culture")
			{
				return c.Value == query.PrimaryCulture;
			}
			if (c.Name == "culture_group")
			{
				return c.Value == query.CultureGroup;
			}
			if (c.Name == "religion")
			{
				return c.Value == query.Religion;
			}
			if (c.Name == "religion_group")
			{
				return c.Value == query.ReligionGroup;
			}
			if (c.Name == "region")
			{
				return c.Value == query.CapitalScopeRegion;
			}
			if (c.Name == "technology_group")
			{
				return query.TechnologyGroup?.Contains(c.Value) ?? false;
			}
			if (c.Name == "government_reform")
			{
				return query.Reforms?.Contains(c.Value) ?? false;
			}
			return false;
		}

		private static bool ResolveConditionSet(TriggerConditionSet cs, CountryIdeaQuery query)
		{
			var returnValue = false;
			foreach (var c in cs.Conditions)
			{
				if (ResolveCondition(c, query))
				{
					if (cs.ComposeOr)
					{
						// OR condition and one returned true;
						// The condition set resolves to true
						return !cs.ModifierNot;
					}
				}
				else
				{
					if (!cs.ComposeOr)
					{
						// AND condition and one returned false;
						return cs.ModifierNot;
					}
				}
			}
			foreach (var subCs in cs.ConditionSets)
			{
				if (cs.ComposeOr && ResolveConditionSet(subCs, query))
				{
					// OR condition and one returned true;
					return !cs.ModifierNot;
				}
				else if (!cs.ComposeOr && !ResolveConditionSet(subCs, query))
				{
					// AND condition and one returned false;
					return cs.ModifierNot;
				}
			}
			return cs.ComposeOr ? cs.ModifierNot : !cs.ModifierNot;
		}
	}

	class CountryIdeaQuery
	{
		public string Tag { get; set; }
		public string PrimaryCulture { get; set; }
		public string CultureGroup { get; set; }
		public string Religion { get; set; }
		public string ReligionGroup { get; set; }
		public string CapitalScopeRegion { get; set; }
		public IReadOnlyCollection<string> TechnologyGroup { get; set; }
		public IReadOnlyCollection<string> Reforms { get; set; }

	}
}
