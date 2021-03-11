using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
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

        #endregion

        public const int MaxLength = 100;
        public const string FileLocation = @".\database.json";
        public const string BackupFolder = @".\Backup";

        private RifaItems _items = new RifaItems();
        private readonly Timer _timer = new Timer();
        private readonly object _lock = new object();

        public IEnumerable<RifaItem> Items
        {
            get
            {
                lock (_lock)
                    return _items.Items;
            }
        }

        private DataBase()
        {
            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);

            _timer.Elapsed += delegate { this.Backup(); };
            _timer.Interval = 1000 * 60 * 60;
            _timer.Start();
        }
        
        public void Load()
        {
            using (StreamReader file = File.OpenText(FileLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                _items = (RifaItems)serializer.Deserialize(file, typeof(RifaItems));
            }

            if (_items.Items.Count == MaxLength)
            {
                this.Backup();
                return;
            }

            int count = _items.Items.Count + 1;
            while (count <= MaxLength)
            {
                _items.Items.Add(new RifaItem { Id = count });
                count++;
            }

            this.Save();
            this.Backup();
        }

        public void Save()
        {
            File.WriteAllText(FileLocation, JsonConvert.SerializeObject(_items, Formatting.Indented));
        }

        public async Task<bool> Save(RifaItem item)
        {
            return await TaskEx.Run(() => SaveItem(item));
        }

        private bool SaveItem(RifaItem item)
        {
            lock (_lock)
            {
                this._items.Items[item.Id - 1] = item;
                this.Save();
            }

            return true;
        }

        private void Backup()
        {
            string file = string.Format("{0}{1}{2}{3}", 
                Path.GetFileNameWithoutExtension(FileLocation),
                "_",
                DateTime.Now.ToString("HHmmss"),
                Path.GetExtension(FileLocation)
                );

            string path = Path.Combine(
                BackupFolder,
                DateTime.Now.ToString("yyyyMMdd")
            );

            string fullFile = Path.Combine(path, file);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            lock (_lock)
            {
                File.Copy(FileLocation, fullFile);
            }
        }
    }
}