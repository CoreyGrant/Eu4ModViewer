using Eu4ModViewer.ModParser.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eu4ModViewer.ModParser.Parser
{
	interface IClauzwitzToJsonFolderConverter
	{
		ParseJsonResult Convert(string baseFolder, string[] excludeFiles);
	}

	class ClauzwitzToJsonFolderConverter : IClauzwitzToJsonFolderConverter
	{
		private const string SearchPattern = "*.txt";
		private readonly IClauzwitzToJsonConverter _converter;
		private readonly IClauzwitzYamlLoader _yamlLoader;

		public ClauzwitzToJsonFolderConverter(
			IClauzwitzToJsonConverter converter,
			IClauzwitzYamlLoader yamlLoader)
		{
			_converter = converter;
			_yamlLoader = yamlLoader;
		}

		public ParseJsonResult Convert(string baseFolder, string[] excludeFiles)
		{
			var commonPath = Path.Combine(baseFolder, "common");
			var historyPath = Path.Combine(baseFolder, "history");
			var eventsPath = Path.Combine(baseFolder, "events");
			var result = new ParseJsonResult();
			var localizationPath = Path.Combine(baseFolder, "localisation");
			//var commonFiles = Directory.GetFiles(commonPath, SearchPattern);
			//var historyFiles = Directory.GetFiles(historyPath, SearchPattern);
			var commonDirectories = Directory.GetDirectories(commonPath);
			result.CommonFiles = ConvertFilesInFolder(baseFolder, "common", excludeFiles);
			foreach (var dir in commonDirectories)
			{
				var dirName = dir.Split("\\").Last();
				result.CommonFolders[dirName] = ConvertFilesInFolder(baseFolder, Path.Combine("common", dirName), excludeFiles);
			}
			if (Directory.Exists(localizationPath))
			{
				var localizations = _yamlLoader.LoadLocalizations(localizationPath);
				result.Localisations = localizations;
			}
			if (Directory.Exists(historyPath))
			{
				var historyDirectories = Directory.GetDirectories(historyPath);
				result.HistoryFiles = ConvertFilesInFolder(baseFolder, "history", excludeFiles);
				foreach (var dir in historyDirectories)
				{
					var dirName = dir.Split("\\").Last();
					result.HistoryFolders[dirName] = ConvertFilesInFolder(baseFolder, Path.Combine("history", dirName), excludeFiles);
				}
			}
			if (Directory.Exists(eventsPath))
			{
				var eventDirectory = Directory.GetDirectories(eventsPath);
				result.EventsFiles = ConvertFilesInFolder(baseFolder, "events", excludeFiles);
				foreach (var dir in eventDirectory)
				{
					var dirName = dir.Split("\\").Last();
					result.EventsFolders[dirName] = ConvertFilesInFolder(baseFolder, Path.Combine("events", dirName), excludeFiles);
				}
			}
			return result;
		}

		private Dictionary<string, JObject> ConvertFilesInFolder(string folderPath, string folderName, string[] excludeFiles)
		{
			var folder = Path.Combine(folderPath, folderName);
			var files = Directory.GetFiles(folder, SearchPattern)
				.Select(x => x.Split("\\").Last())
				.Where(x => !excludeFiles.Any(y => x.Contains(y)));
			var outputDict = new Dictionary<string, JObject>();
			foreach (var file in files)
			{
				var obj = _converter.ParseObject(file, folder);
				outputDict[file] = obj;
			}
			return outputDict;
		}
	}
}
