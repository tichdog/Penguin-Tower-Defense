using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int saveVersion = SaveSystem.SAVE_VERSION;
        public string saveDate = DateTime.UtcNow.ToString("o");
        public string deviceId = "";
        public List<SaveRecord> records = new List<SaveRecord>();

        public void Normalize()
        {
            if (records == null)
                records = new List<SaveRecord>();
        }

        public void SetData<T>(string key, T value)
        {
            ValidateKey(key);

            if (!typeof(T).IsValueType && EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                RemoveData(key);
                return;
            }

            SaveRecord record = FindRecord(key);
            if (record == null)
            {
                record = new SaveRecord { key = key };
                records.Add(record);
            }

            record.type = typeof(T).AssemblyQualifiedName;
            record.json = JsonUtility.ToJson(value, false);
            record.version = SaveSystem.SAVE_VERSION;
            record.updatedAt = DateTime.UtcNow.ToString("o");
        }

        public bool TryGetData<T>(string key, out T value)
        {
            value = default;
            SaveRecord record = FindRecord(key);

            if (record == null || string.IsNullOrEmpty(record.json))
                return false;

            try
            {
                value = JsonUtility.FromJson<T>(record.json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveSystem] Failed to read save record '{key}' as {typeof(T).Name}: {ex.Message}");
                value = default;
                return false;
            }
        }

        public T GetData<T>(string key, T fallback = default)
        {
            return TryGetData(key, out T value) ? value : fallback;
        }

        public bool HasData(string key)
        {
            return FindRecord(key) != null;
        }

        public bool RemoveData(string key)
        {
            SaveRecord record = FindRecord(key);
            if (record == null)
                return false;

            records.Remove(record);
            return true;
        }

        private SaveRecord FindRecord(string key)
        {
            Normalize();

            for (int i = 0; i < records.Count; i++)
            {
                SaveRecord record = records[i];
                if (record != null && record.key == key)
                    return record;
            }

            return null;
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Save data key cannot be empty.", nameof(key));
        }
    }

    [Serializable]
    public class SaveRecord
    {
        public string key = "";
        public string type = "";
        public int version = SaveSystem.SAVE_VERSION;
        public string updatedAt = "";
        public string json = "";
    }
}
