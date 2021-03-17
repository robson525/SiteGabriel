using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rifa.Models;

namespace Rifa.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private const string SessionInfoId = "SiteGabrielAdmin";

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RifaItem>))]
        public async Task<IActionResult> CheckLogged()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionInfoId)))
                return BadRequest();

            return Ok(DataBase.Instance.Items);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RifaItem>))]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                Logger.Instance.WriteError($"ModelState Is not Valid");
                return BadRequest(ModelState);
            }

            if (login.User == Mail.From && login.Password == Mail.Password)
            {
                HttpContext.Session.SetString(SessionInfoId, HttpContext.Session.Id);
                return Ok(DataBase.Instance.Items);
            }

            Logger.Instance.WriteWarning($"Invalid login: {login}");
            return Unauthorized();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RifaItem))]
        public async Task<IActionResult> GetItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                Logger.Instance.WriteError($"ModelState Is not Valid");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionInfoId)))
            {
                Logger.Instance.WriteWarning($"Session is empty");
                return Unauthorized();
            }

            RifaItem item = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (item == null)
            {
                Logger.Instance.WriteError($"Id '{id}' didn't exist");
                return BadRequest();
            }

            return Ok(item);
        }
    }
}
