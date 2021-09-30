using Eu4ModViewer.Shared.Api;
using Eu4ModViewer.Shared.Database;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eu4ModViewer.Controllers
{
	[ApiController]
	[Route("api/Mod")]
	public class ModController : ControllerBase
	{
		private readonly IModService modService;
		private readonly ISteamApiFacade steamApiFacade;

		public ModController(IModService modService, ISteamApiFacade steamApiFacade)
		{
			this.modService = modService;
			this.steamApiFacade = steamApiFacade;
		}

		[HttpGet("")]
		[ResponseCache(Duration = 60)]
		public IActionResult GetMods()
		{
			return this.Ok(modService.GetMods());
		}

		[HttpGet("{id}")]
		[ResponseCache(Duration = 60)]

		public IActionResult GetModDetails(long id)
		{
			return this.Ok(this.modService.GetMod(id));
		}

		[HttpGet("PublishedFileDetails/{id}")]
		public async Task<IActionResult> PublishedFileDetails(long id)
		{
			return this.Ok(await this.steamApiFacade.GetPublishedFileDetails(id));
		}
		
	}
}
