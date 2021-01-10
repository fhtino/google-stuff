using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace AuthCommonLib
{

    public class DemoStorage
    {

        // *************************************
        // !!! This is not a secure approach !!!
        // !!! Only for demo purpose         !!!
        // *************************************


        public class StorageItem
        {
            public string GoogleID { get; set; }            
            public string RefreshToken { get; set; }
            public DateTime UpdateDT { get; set; }
        }



        private string _jsonFileName;

        //private List<StorageItem> _items { get; set; }


        public DemoStorage(string jsonFileName)
        {
            _jsonFileName = jsonFileName;

            if (!File.Exists(_jsonFileName))
            {
                Save(new List<StorageItem>());
            }           
        }

        private List<StorageItem> Load()
        {
            return JsonConvert.DeserializeObject<List<StorageItem>>(File.ReadAllText(_jsonFileName));
        }

        private void Save(List<StorageItem> items)
        {
            File.WriteAllText(_jsonFileName, JsonConvert.SerializeObject(items));
        }


        public void Store(string googleID, string refreshToken)
        {
            var items = Load();

            var item = items.SingleOrDefault(x => x.GoogleID == googleID);

            if (item == null)
            {
                item = new StorageItem() { GoogleID = googleID };
                items.Add(item);
            }

            item.RefreshToken = refreshToken;
            item.UpdateDT = DateTime.UtcNow;

            Save(items);
        }


        public string[] GetGoogleIDs()
        {
            return Load().Select(x => x.GoogleID).ToArray();
        }


        public string GetRefreshToken(string googleID)
        {
            return Load().Single(x => x.GoogleID == googleID).RefreshToken;
        }


    }

}
