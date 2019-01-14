using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ILuvLuis.Web.Controllers
{
    [Route("bizzarre")]
    public class BizzarreController : Controller
    {
        private readonly ILogger<BizzarreController> _logger;

        public BizzarreController(ILogger<BizzarreController> logger)
        {
            _logger = logger;
        }

        [HttpGet("nice")]
        public IActionResult Holis() => Ok("esto funca");

        [HttpGet("bad")]
        public IActionResult Goodbye() => throw new Exception("asd");
    }
}
