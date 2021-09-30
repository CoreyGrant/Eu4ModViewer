using Eu4ModViewer.Shared.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer.Controllers
{
	[ApiController]
	[Route("api/File")]
	public class FileController : ControllerBase
	{
		private readonly ICloudFileService cloudFileService;

		public FileController(ICloudFileService cloudFileService)
		{
			this.cloudFileService = cloudFileService;
		}

		[HttpGet("Image/{filename}/{folder}/{modId}")]
		public async Task<IActionResult> GetImage(string filename, string folder, long? modId)
		{
			var fileResult = await cloudFileService.GetImageFileAsync(filename, modId, folder);
			return File(fileResult, "image/png");
		}

		[HttpGet("Json/{filename}/{modId}")]
		public async Task<IActionResult> GetJson(string filename, long? modId)
		{
			var fileResult = await cloudFileService.GetFileAsync(filename, modId);
			return File(fileResult, "application/json");
		}
	}
}
