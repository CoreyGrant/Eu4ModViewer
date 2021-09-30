using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Eu4ModViewer.Shared.Entities
{
	public class ModQueue
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ModQueueId { get; set; }
		public long ModId { get; set; }
		public int Status { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime LastUpdated { get; set; }
		public string ExcludeFiles { get; set; }
	}
}
