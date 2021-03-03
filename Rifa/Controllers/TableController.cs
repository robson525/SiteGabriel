using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rifa.Models;

namespace Rifa.Controllers
{
    [Route("api/[controller]")]
    public class TableController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<RifaItem> LoadRifaItems()
        {
            return Enumerable.Range(1, 100)
                .Select(index => new RifaItem
                {
                    Number = index
                });
        }
    }
}
