using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Eu4ModViewer.Shared.Entities
{
	public class Log
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int LogId { get; set; }
		public int Type { get; set; }
		public string Message { get; set; }
	}
}
