using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eu4ModViewer.Shared.Database
{
	public interface ILogService
	{
		void Log(string message, LogLevel level);
	}

	public class LogService : ILogService
	{
		private readonly Eu4ModViewerDatabase _database;

		public LogService(Eu4ModViewerDatabase database)
		{
			_database = database;
		}

		public void Log(string message, LogLevel level)
		{
			_database.Log.Add(new Entities.Log { Message = message, Type = (int)level });
			_database.SaveChanges();
		}
	}
}
