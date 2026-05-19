using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }
        public static SaveData Data => Instance != null ? Instance._current : null;

        [Header("Save Slots")]
        [SerializeField] private int totalSlots = 3;
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60f;
        [SerializeField] private bool backupOnSave = true;

        [Header("Debug")]
        [SerializeField] private bool verboseLog = true;

        public static event Action<int> OnSaved;
        public static event Action<int> OnLoaded;
        public static event Action<string> OnError;

        private SaveData _current;
        private int _activeSlot = 0;
        private Coroutine _autoSaveCo;

        private string Secret => SystemInfo.deviceUniqueIdentifier + "_8383210943323";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _current = NewSave();
        }

        private void Start()
        {
            if (loadOnStart)
                Load(_activeSlot);

            if (autoSave)
                _autoSaveCo = StartCoroutine(AutoSaveRoutine());
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                Save(_activeSlot);
        }

        private void OnApplicationQuit()
        {
            Save(_activeSlot);
        }

        public void Save() => Save(_activeSlot);

        public void Save(int slot)
        {
            try
            {
                ValidateSlot(slot);
                EnsureCurrent();

                _current.Normalize();
                _current.saveDate = DateTime.UtcNow.ToString("o");
                _current.deviceId = SystemInfo.deviceUniqueIdentifier;

                string path = GetPath(slot);
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                if (backupOnSave && File.Exists(path))
                    File.Copy(path, path + ".bak", true);

                byte[] bytes = SaveSystem.Serialize(_current, Secret);
                File.WriteAllBytes(path, bytes);

                _activeSlot = slot;
                Log($"Saved slot {slot}: {path}");
                OnSaved?.Invoke(slot);
            }
            catch (Exception ex)
            {
                LogError($"Failed to save slot {slot}: {ex.Message}");
                OnError?.Invoke(ex.Message);
            }
        }

        public bool Load() => Load(_activeSlot);

        public bool Load(int slot)
        {
            ValidateSlot(slot);
            string path = GetPath(slot);

            if (!File.Exists(path))
            {
                _current = NewSave();
                _activeSlot = slot;
                Log($"Slot {slot} is empty. New save data created.");
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                SaveData data = SaveSystem.Deserialize(bytes, Secret);
                ApplyLoadedData(data, slot);
                Log($"Loaded slot {slot}.");
                return true;
            }
            catch (SaveCorruptedException ex)
            {
                LogError($"Slot {slot} is corrupted: {ex.Message}");
                OnError?.Invoke($"Save is corrupted: {ex.Message}");
                return TryLoadBackup(slot);
            }
            catch (Exception ex)
            {
                LogError($"Failed to load slot {slot}: {ex.Message}");
                OnError?.Invoke(ex.Message);
                _current = NewSave();
                return false;
            }
        }

        public void DeleteSave(int slot)
        {
            ValidateSlot(slot);
            string path = GetPath(slot);

            if (File.Exists(path))
                File.Delete(path);

            if (File.Exists(path + ".bak"))
                File.Delete(path + ".bak");

            if (slot == _activeSlot)
                _current = NewSave();

            Log($"Deleted slot {slot}.");
        }

        public bool SlotExists(int slot)
        {
            ValidateSlot(slot);
            return File.Exists(GetPath(slot));
        }

        public DateTime? GetSlotDate(int slot)
        {
            try
            {
                if (!SlotExists(slot))
                    return null;

                byte[] bytes = File.ReadAllBytes(GetPath(slot));
                SaveData data = SaveSystem.Deserialize(bytes, Secret);
                return DateTime.Parse(data.saveDate);
            }
            catch
            {
                return null;
            }
        }

        public void SetData<T>(string key, T value)
        {
            EnsureCurrent();
            _current.SetData(key, value);
        }

        public bool TryGetData<T>(string key, out T value)
        {
            EnsureCurrent();
            return _current.TryGetData(key, out value);
        }

        public T GetData<T>(string key, T fallback = default)
        {
            EnsureCurrent();
            return _current.GetData(key, fallback);
        }

        public bool HasData(string key)
        {
            EnsureCurrent();
            return _current.HasData(key);
        }

        public bool RemoveData(string key)
        {
            EnsureCurrent();
            return _current.RemoveData(key);
        }

        public void UpdateData<T>(string key, Action<T> update) where T : new()
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            EnsureCurrent();
            T value = _current.TryGetData(key, out T loaded) ? loaded : new T();
            update(value);
            _current.SetData(key, value);
        }

        public void SaveBlock<T>(string key, T value, int slot = -1)
        {
            SetData(key, value);
            Save(slot >= 0 ? slot : _activeSlot);
        }

        public bool LoadBlock<T>(string key, out T value, int slot = -1)
        {
            if (slot >= 0 && slot != _activeSlot)
                Load(slot);

            return TryGetData(key, out value);
        }

        private IEnumerator AutoSaveRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveInterval);
                Save(_activeSlot);
            }
        }

        private bool TryLoadBackup(int slot)
        {
            string backupPath = GetPath(slot) + ".bak";
            if (!File.Exists(backupPath))
            {
                _current = NewSave();
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(backupPath);
                SaveData data = SaveSystem.Deserialize(bytes, Secret);
                ApplyLoadedData(data, slot);
                LogError($"Loaded backup for slot {slot}.");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to load backup for slot {slot}: {ex.Message}");
                _current = NewSave();
                return false;
            }
        }

        private void ApplyLoadedData(SaveData data, int slot)
        {
            _current = data ?? NewSave();
            _current.Normalize();
            _activeSlot = slot;

            OnLoaded?.Invoke(slot);
        }

        private SaveData NewSave()
        {
            SaveData data = new SaveData();
            data.deviceId = SystemInfo.deviceUniqueIdentifier;
            data.Normalize();
            return data;
        }

        private void EnsureCurrent()
        {
            if (_current == null)
                _current = NewSave();

            _current.Normalize();
        }

        private string GetPath(int slot)
        {
            return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.dat");
        }

        private void ValidateSlot(int slot)
        {
            if (slot < 0 || slot >= totalSlots)
                throw new ArgumentOutOfRangeException(nameof(slot), $"Slot {slot} is invalid. Total slots: {totalSlots}");
        }

        private void Log(string message)
        {
            if (verboseLog)
                Debug.Log($"[SaveSystem] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[SaveSystem] {message}");
        }
    }
}
