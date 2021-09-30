using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer.Models.Queue
{
	public class QueueModRequest
	{
		public long ModId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime LastUpdated { get; set; }
		public string ExcludeFiles { get; set; }
	}
}
