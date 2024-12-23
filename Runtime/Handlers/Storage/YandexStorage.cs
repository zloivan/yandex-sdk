using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Yandex.Helpers;

namespace Yandex.Handlers
{
    public class YandexStorage
    {
        private readonly Dictionary<string, object> _cachedData = new();
        private readonly ILogger _logger = new YandexSDKLogger();
        private bool _isDirty;
        private bool _isDataLoaded;
        private float _autoSaveInterval = 30f;
        private float _lastSaveTime;

        public event Action<bool> DataSaved;
        public event Action DataLoaded;

        [DllImport("__Internal")]
        private static extern void SaveDataInternal(string data);

        [DllImport("__Internal")]
        private static extern void LoadDataInternal();

        [DllImport("__Internal")]
        private static extern void DeleteAllInternal();

        internal void Initialize()
        {
            _logger.Log("YANDEX_STORAGE", "Initializing");
            if (!Application.isEditor)
            {
                LoadDataInternal();
            }
            else
            {
                _isDataLoaded = true;
            }
        }

        public void SetInt(string key, int value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting int: {key} = {value}");
            if (Application.isEditor)
            {
                PlayerPrefs.SetInt(key, value);
            }

            SetValue(key, value);
        }

        public void SetFloat(string key, float value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting float: {key} = {value}");
            if (Application.isEditor)
            {
                PlayerPrefs.SetFloat(key, value);
            }

            SetValue(key, value);
        }

        public void SetString(string key, string value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting string: {key} = {value}");
            if (Application.isEditor)
            {
                PlayerPrefs.SetString(key, value);
            }

            SetValue(key, value);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (Application.isEditor)
            {
                return PlayerPrefs.GetInt(key, defaultValue);
            }

            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is int value)
                return value;
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            if (Application.isEditor)
            {
                return PlayerPrefs.GetFloat(key, defaultValue);
            }

            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is float value)
                return value;
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            if (Application.isEditor)
            {
                return PlayerPrefs.GetString(key, defaultValue);
            }

            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is string value)
                return value;
            return defaultValue;
        }

        public bool HasKey(string key)
        {
            if (Application.isEditor)
            {
                return PlayerPrefs.HasKey(key);
            }

            EnsureDataLoaded();
            return _cachedData.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            _logger.Log("YANDEX_STORAGE", $"Deleting key: {key}");

            if (Application.isEditor)
            {
                PlayerPrefs.DeleteKey(key);
            }

            if (_cachedData.ContainsKey(key))
            {
                _cachedData.Remove(key);
                _isDirty = true;
            }
        }

        public void DeleteAll()
        {
            _logger.Log("YANDEX_STORAGE", "Deleting all data");
            _cachedData.Clear();
            _isDirty = false;

            if (Application.isEditor)
            {
                PlayerPrefs.DeleteAll();
            }
            else
            {
                DeleteAllInternal();
            }
        }

        internal void Update()
        {
            if (_isDirty && Time.time - _lastSaveTime >= _autoSaveInterval)
            {
                Save();
            }
        }

        internal void OnDataSaved(bool success)
        {
            _logger.Log("YANDEX_STORAGE", $"Data saved: {success}");
            DataSaved?.Invoke(success);
        }

        internal void OnDataLoaded(string jsonData)
        {
            if (Application.isEditor) return; // Shouldn't be called in editor mode

            _logger.Log("YANDEX_STORAGE", "Data loaded");

            try
            {
                var storageData = JsonUtility.FromJson<StorageData>(jsonData);
                if (storageData != null)
                {
                    _cachedData.Clear();
                    var dict = storageData.ToDictionary();
                    foreach (var kvp in dict)
                    {
                        _cachedData[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("YANDEX_STORAGE", $"Error parsing loaded data: {e.Message}");
            }

            _isDataLoaded = true;
            _lastSaveTime = Time.time;
            DataLoaded?.Invoke();
        }

        public void Save()
        {
            if (!_isDirty) return;

            _logger.Log("YANDEX_STORAGE", "Saving data");

            if (Application.isEditor)
            {
                PlayerPrefs.Save();
            }
            else
            {
                var storageData = StorageData.FromDictionary(_cachedData);
                var json = JsonUtility.ToJson(storageData);
                SaveDataInternal(json);
            }

            _lastSaveTime = Time.time;
            _isDirty = false;
        }

        private void SetValue(string key, object value)
        {
            EnsureDataLoaded();
            _cachedData[key] = value;
            _isDirty = true;
        }

        private void EnsureDataLoaded()
        {
            if (_isDataLoaded) return;

            if (!Application.isEditor)
            {
                LoadDataInternal();
            }
            else
            {
                _isDataLoaded = true;
            }
        }
    }
}