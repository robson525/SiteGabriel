using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rifa.Models;

namespace Rifa.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        [HttpGet]
        public IEnumerable<RifaItem> GetItems()
        {
            return DataBase.Instance.Items;
        }
        
        [HttpGet("{id}")]
        public RifaItem GetItem([FromRoute] int id)
        {
            RifaItem item = DataBase.Instance.Items.FirstOrDefault(_ => _.Id == id);
            if (item == null) return new RifaItem();

            if(item.Status == RifaItem.ItemStatus.Idle)
                item.Status = RifaItem.ItemStatus.Reserving;

            DataBase.Instance.Save(item);
            return item;

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Save([FromRoute] int id, [FromBody] RifaItem item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != item.Id)
            {
                return BadRequest();
            }

            item.Status = RifaItem.ItemStatus.Reserved;
            DataBase.Instance.Save(item);
            return Ok();
        }
    }
}
