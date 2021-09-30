using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Models.Shared
{
	/// <summary>
	/// The trigger is made up of one or more condition sets.
	/// </summary>
	public class Trigger
	{
		public List<TriggerCondition> Conditions { get; set; }
		public List<TriggerConditionSet> ConditionSets { get; set; }
	}

	public class TriggerConditionSet
	{
		public bool ComposeOr { get; set; }
		public bool ModifierNot { get; set; }
		public List<TriggerCondition> Conditions { get; set; }
		public List<TriggerConditionSet> ConditionSets { get; set; }
	}

	public class TriggerCondition
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
