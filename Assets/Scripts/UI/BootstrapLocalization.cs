using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class BootstrapLocalization : MonoBehaviour
{
    private async void Awake()
    {
        await LocalizationSettings.InitializationOperation.Task;

        SaveData data = SaveManager.Load();

        if (data == null)
            return;

        Locale locale =
            LocalizationSettings.AvailableLocales
            .GetLocale(data.languageCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }
}
