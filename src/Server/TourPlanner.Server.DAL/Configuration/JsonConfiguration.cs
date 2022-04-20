using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Server.DAL.Configuration
{
    public class JsonConfiguration : IConfiguration
    {
        private readonly Dictionary<string, object> _configuration = new();
        private readonly string _savePath;

        public JsonConfiguration(string saveFolderPath)
        {
            _savePath = Path.Combine(saveFolderPath, "config.json");

            try
            {
                var file = File.ReadAllText(_savePath);

                var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(file);
                if (_configuration == null)
                    _configuration = new();
            }
            catch
            {
                _configuration = new();
            }
        }

        private void SaveJson()
        {
            try
            {
                File.WriteAllText(_savePath, JsonConvert.SerializeObject(_configuration, Formatting.Indented));
            }
            catch
            {

            }
        }

        public void AddObject(string name, object value)
        {
            _configuration.Add(name, value);
            SaveJson();
        }

        public bool TryGetObject(string name, out object? obj)
        {
            return _configuration.TryGetValue(name, out obj);
        }

        public void RemoveObject(string name)
        {
            _configuration.Remove(name);
            SaveJson();
        }

        public void UpdateObject(string name, object value)
        {
            if (_configuration.ContainsKey(name))
            {
                _configuration[name] = value;
                SaveJson();
            }
        }
    }
}
