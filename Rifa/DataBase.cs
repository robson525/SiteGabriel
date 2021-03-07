using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rifa.Models;

namespace Rifa
{
    public class DataBase
    {
        #region Singleton

        private static DataBase _instance;

        public static DataBase Instance => _instance ?? (_instance = new DataBase());

        private DataBase() { }

        #endregion

        public const int MaxLength = 100;
        public const string FileLocation = @".\database.json";

        private RifaItems _items = new RifaItems();
        private readonly object _lock = new object();

        public IEnumerable<RifaItem> Items
        {
            get
            {
                lock (_lock)
                    return _items.Items;
            }
        }

        public void Load()
        {
            using (StreamReader file = File.OpenText(FileLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                _items = (RifaItems)serializer.Deserialize(file, typeof(RifaItems));
            }

            if (_items.Items.Count == MaxLength)
                return;

            int count = _items.Items.Count + 1;
            while (count <= MaxLength)
            {
                _items.Items.Add(new RifaItem { Id = count });
                count++;
            }

            this.Save();
        }

        private void Save()
        {
            File.WriteAllText(FileLocation, JsonConvert.SerializeObject(_items, Formatting.Indented));
        }

        public bool Save(RifaItem item)
        {
            lock (_lock)
            {
                this._items.Items[item.Id - 1] = item;
                this.Save();
            }

            return true;
        }
    }
}