using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Eu4ModViewer.Shared.Entities
{
	public class Mod
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public long ModId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
