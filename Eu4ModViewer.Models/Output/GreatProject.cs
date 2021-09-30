using Eu4ModViewer.Models.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Output
{
	public class GreatProject
	{
		public string Name { get; set; }
		public int Start { get; set; }
		public string StartName { get; set; }
		public int BuildCost { get; set; }
		public bool CanBeMoved { get; set; }
		public int StartingTier { get; set; }
		public string Type { get; set; }
		public Trigger BuildTrigger { get; set; }
		public Trigger CanUseModifiersTrigger { get; set; }
		public Trigger CanUpgradeTrigger { get; set; }
		public Trigger KeepTrigger { get; set; }
		public Tier Tier0 { get; set; }
		public Tier Tier1 { get; set; }
		public Tier Tier2 { get; set; }
		public Tier Tier3 { get; set; }
	}

	public class Tier
	{
		public Time UpgradeTime { get; set; }
		public Factor CostToUpgrade { get; set; }
		public IReadOnlyDictionary<string, string> ProvinceModifiers { get; set; }
		public IReadOnlyDictionary<string, string> AreaModifier { get; set; }
		public IReadOnlyDictionary<string, string> CountryModifiers { get; set; }
	}

	public class Factor
	{
		public int Value { get; set; }
	}

	public class Time
	{
		public int Months { get; set; }
	}
}
