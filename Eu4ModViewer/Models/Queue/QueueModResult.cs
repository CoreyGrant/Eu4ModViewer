using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer.Models.Queue
{
	public class QueueModResult
	{
		public int? ModQueueId { get; set; }
		public bool Success { get; set; }
	}
}
