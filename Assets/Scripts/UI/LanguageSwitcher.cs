using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    // Храним языки из Localization
    private List<Locale> locales = new();
    // Для Dropdown опций (нужные нам названия и их ключи) 
    private Dictionary<string, string> languageNames = new()
    {
        { "en", "English" },
        { "ru", "Русский" },
        { "de", "German" },
    };

    private async void Start()
    {
        // Ждем загрузки localization
        await LocalizationSettings.InitializationOperation.Task;
        // Забираем все языки из localization
        locales = LocalizationSettings.AvailableLocales.Locales;

        dropdown.ClearOptions();

        List<string> options = new();
        // Для текущего языка
        int currentIndex = 0;

        for (int i = 0; i < locales.Count; i++)
        {
            Locale locale = locales[i];

            string code = locale.Identifier.Code;
            // Ищем по ключу
            if (languageNames.TryGetValue(code, out string displayName))
                options.Add(displayName);
            else
                options.Add(locale.LocaleName);
            // Сравниваем с текущим языком 
            if (LocalizationSettings.SelectedLocale == locale)
                currentIndex = i;
        }

        dropdown.AddOptions(options);

        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();
        // Подписка на событие изменения языка
        dropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    private void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = locales[index];
    }
}
