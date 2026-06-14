using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    [SerializeField] private LocalizedString coinsLocalizedString;

    private void OnEnable()
    {
        if (coinsLocalizedString != null)
            coinsLocalizedString.StringChanged += UpdateText;

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnCoinsChanged += OnCoinsChanged;
            OnCoinsChanged(EconomyManager.Instance.Coins);
        }
    }

    private void OnDisable()
    {
        if (coinsLocalizedString != null)
            coinsLocalizedString.StringChanged -= UpdateText;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int coins)
    {
        if (coinsLocalizedString == null)
        {
            UpdateText(coins.ToString());
            return;
        }

        coinsLocalizedString.Arguments = new object[]
        {
            coins
        };

        coinsLocalizedString.RefreshString();
    }

    private void UpdateText(string value)
    {
        if (coinsText != null)
            coinsText.text = value;
    }
}
