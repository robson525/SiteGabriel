using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore;
using Rifa.Models;

namespace Rifa.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private const string SessionInfoId = "SiteGabriel";

        [HttpGet]
        public IEnumerable<RifaItem> GetItems()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionInfoId)))
                HttpContext.Session.SetString(SessionInfoId, HttpContext.Session.Id);

            return DataBase.Instance.Items;
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

            RifaItem item = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (item == null)
            {
                Logger.Instance.WriteError($"Id '{id}' didn't exist");
                return BadRequest();
            }

            if (item.Status != RifaItem.ItemStatus.Idle)
            {
                if (item.Status > RifaItem.ItemStatus.Idle && item.SessionId != HttpContext.Session.Id)
                {
                    Logger.Instance.WriteWarning($"Invalid state. Session '{HttpContext.Session.Id}' trying get: {item}");
                    return Unauthorized();
                }
            }

            if (item.Status == RifaItem.ItemStatus.Idle)
            {
                Logger.Instance.WriteNone($"Session '{HttpContext.Session.Id}' reserving item: {item}");
                item.SetStatus(RifaItem.ItemStatus.Reserving);
            }

            item.SessionId = HttpContext.Session.Id;
            await DataBase.Instance.Save(item);
            return Ok(item);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Save([FromRoute] int id, [FromBody] RifaItem item)
        {
            if (!ModelState.IsValid)
            {
                Logger.Instance.WriteError($"ModelState Is not Valid");
                return BadRequest(ModelState);
            }
            if (id != item.Id)
            {
                Logger.Instance.WriteError($"Trying to save a invalid id. Url: '{id}'. Body '{item.Id}'.");
                return BadRequest();
            }

            RifaItem itemDb = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (itemDb == null)
            {
                Logger.Instance.WriteError($"Id '{id}' didn't exist");
                return BadRequest();
            }

            if (itemDb.Status > RifaItem.ItemStatus.Reserved || itemDb.SessionId != HttpContext.Session.Id)
            {
                Logger.Instance.WriteWarning($"Invalid state. Session '{HttpContext.Session.Id}' trying save: {item}");
                return Unauthorized();
            }

            Logger.Instance.WriteNone($"Session '{HttpContext.Session.Id}' reserved item: {item}");

            item.SessionId = itemDb.SessionId;
            item.SetStatus(itemDb.Status == RifaItem.ItemStatus.Reserving ? RifaItem.ItemStatus.Reserved : itemDb.Status);
            Mail.Instance.Reserved(item);

            await DataBase.Instance.Save(item);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (item == null) return BadRequest();

            if (item.Status != RifaItem.ItemStatus.Reserving || item.SessionId != HttpContext.Session.Id)
                return Unauthorized();

            item.SetStatus(RifaItem.ItemStatus.Idle);
            await DataBase.Instance.Save(item);
            return Ok();
        }
    }
}
