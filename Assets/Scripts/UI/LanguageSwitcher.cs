using System.Collections.Generic;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    private const string SaveKey = SaveKeys.SettingsLanguage;

    [SerializeField] private TMP_Dropdown dropdown;

    [System.Serializable]
    private class LanguageSaveData
    {
        public string code;
    }

    private List<Locale> locales = new();

    private Dictionary<string, string> languageNames = new()
    {
        { "en", "English" },
        { "ru", "Русский" },
        { "de", "German" },
    };

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;
        locales = LocalizationSettings.AvailableLocales.Locales;

        ApplySavedLanguage();

        dropdown.ClearOptions();

        List<string> options = new();
        int currentIndex = 0;

        for (int i = 0; i < locales.Count; i++)
        {
            Locale locale = locales[i];
            string code = locale.Identifier.Code;

            if (languageNames.TryGetValue(code, out string displayName))
                options.Add(displayName);
            else
                options.Add(locale.LocaleName);

            if (LocalizationSettings.SelectedLocale == locale)
                currentIndex = i;
        }

        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(currentIndex);
        dropdown.RefreshShownValue();
    }

    private void ChangeLanguage(int index)
    {
        if (index < 0 || index >= locales.Count)
            return;

        Locale selectedLocale = locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        string code = selectedLocale.Identifier.Code;
        GameDataManager.Instance.SetData(SaveKey, new LanguageSaveData { code = code });
        GameDataManager.Instance.Save();
    }

    private void OnEnable()
    {
        if (dropdown != null)
            dropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    private void OnDisable()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(ChangeLanguage);
    }

    private void ApplySavedLanguage()
    {
        if (GameDataManager.Instance == null)
            return;

        string savedCode = "";

        if (GameDataManager.Instance.TryGetData(SaveKey, out LanguageSaveData savedLanguage))
            savedCode = savedLanguage.code;

        if (string.IsNullOrEmpty(savedCode))
            return;

        Locale savedLocale = locales.Find(locale => locale.Identifier.Code == savedCode);
        if (savedLocale != null)
            LocalizationSettings.SelectedLocale = savedLocale;
    }
}
