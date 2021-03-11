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
            RifaItem item = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (item == null) return BadRequest();

            if (item.Status != RifaItem.ItemStatus.Idle)
                if (item.Status == RifaItem.ItemStatus.Reserving && item.SessionId != HttpContext.Session.Id)
                    return Unauthorized();

            item.SessionId = HttpContext.Session.Id;
            item.SetStatus(RifaItem.ItemStatus.Reserving);
            await DataBase.Instance.Save(item);
            return Ok(item);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Save([FromRoute] int id, [FromBody] RifaItem item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != item.Id)
                return BadRequest();

            RifaItem itemDB = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (itemDB == null) return BadRequest();

            if (itemDB.Status != RifaItem.ItemStatus.Reserving && itemDB.Status != RifaItem.ItemStatus.Idle || itemDB.SessionId != HttpContext.Session.Id)
                return Unauthorized();

            item.SetStatus(RifaItem.ItemStatus.Reserved);
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

            if (item.Status != RifaItem.ItemStatus.Reserving)
                return Unauthorized();

            item.SetStatus(RifaItem.ItemStatus.Idle);
            await DataBase.Instance.Save(item);
            return Ok();
        }
    }
}
