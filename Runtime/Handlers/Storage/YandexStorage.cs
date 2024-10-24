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

        // Public methods (PlayerPrefs-like API)
        public void SetInt(string key, int value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting int: {key} = {value}");
            SetValue(key, value);
        }

        public void SetFloat(string key, float value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting float: {key} = {value}");
            SetValue(key, value);
        }

        public void SetString(string key, string value)
        {
            _logger.Log("YANDEX_STORAGE", $"Setting string: {key} = {value}");
            SetValue(key, value);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is int value)
                return value;
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is float value)
                return value;
            return defaultValue;
        }

        public string GetString(string key, string defaultValue = "")
        {
            EnsureDataLoaded();
            if (HasKey(key) && _cachedData[key] is string value)
                return value;
            return defaultValue;
        }

        public bool HasKey(string key)
        {
            EnsureDataLoaded();
            return _cachedData.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            _logger.Log("YANDEX_STORAGE", $"Deleting key: {key}");
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
            if (!Application.isEditor)
            {
                DeleteAllInternal(); 
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }

        // Internal methods
        internal void Initialize()
        {
            _logger.Log("YANDEX_STORAGE", "Initializing");
            LoadDataInternal();
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

        private void Save()
        {
            if (!_isDirty) return;

            _logger.Log("YANDEX_STORAGE", "Saving data");

            if (!Application.isEditor)
            {
                var storageData = StorageData.FromDictionary(_cachedData);
                var json = JsonUtility.ToJson(storageData);
                SaveDataInternal(json);
            }
            else
            {
                PlayerPrefs.Save();
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
        }
    }
}