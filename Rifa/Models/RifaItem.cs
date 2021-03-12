using System.Collections.Generic;
using System.Timers;

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

        public string SessionId { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Idle;

        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }

        private readonly Timer _timer;

        public RifaItem()
        {
            _timer = new Timer();
            _timer.Elapsed += Timer_Elapsed;
            _timer.Interval = 1000 * 60 * 20;
        }

        public void SetStatus(ItemStatus status)
        {
            this.Status = status;

            if(status == ItemStatus.Reserving)
                _timer.Start();
            else
                _timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            if (this.Status != ItemStatus.Reserving) return;

            this.SessionId = string.Empty;
            this.Status = ItemStatus.Idle;
            DataBase.Instance.Save();
        }
    }

    public class RifaItems
    {
        public List<RifaItem> Items { get; set; }
    }
}