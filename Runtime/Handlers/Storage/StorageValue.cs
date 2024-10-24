using System;
using System.Collections.Generic;

namespace Yandex.Handlers
{
    [Serializable]
    public class StorageValue
    {
        public string type;
        public string value;

        public static StorageValue FromObject(object obj)
        {
            var storageValue = new StorageValue();
            switch (obj)
            {
                case int intValue:
                    storageValue.type = "int";
                    storageValue.value = intValue.ToString();
                    break;
                case float floatValue:
                    storageValue.type = "float";
                    storageValue.value = floatValue.ToString("R");
                    break;
                case string stringValue:
                    storageValue.type = "string";
                    storageValue.value = stringValue;
                    break;
            }

            return storageValue;
        }

        public object ToObject()
        {
            switch (type)
            {
                case "int": return int.Parse(value);
                case "float": return float.Parse(value);
                case "string": return value;
                default: throw new ArgumentException($"Unknown type: {type}");
            }
        }
    }

    [Serializable]
    public class StorageEntry
    {
        public string key;
        public StorageValue value;
    }

    [Serializable]
    public class StorageData
    {
        public StorageEntry[] entries;

        public static StorageData FromDictionary(Dictionary<string, object> dict)
        {
            var data = new StorageData
            {
                entries = new StorageEntry[dict.Count]
            };

            int index = 0;
            foreach (var kvp in dict)
            {
                data.entries[index] = new StorageEntry
                {
                    key = kvp.Key,
                    value = StorageValue.FromObject(kvp.Value)
                };
                index++;
            }

            return data;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            if (entries == null) return dict;

            foreach (var entry in entries)
            {
                if (entry?.key != null && entry.value != null)
                {
                    dict[entry.key] = entry.value.ToObject();
                }
            }

            return dict;
        }
    }
}