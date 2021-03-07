using System.Collections.Generic;

namespace Rifa.Models
{
    public class RifaItem
    {
        public enum ItemStatus
        {
            Idle,
            Reserving,
            Reserved,
            Paid
        }

        public int Id { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Idle;

        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }
    }

    public class RifaItems
    {
        public List<RifaItem> Items { get; set; }
    }
}