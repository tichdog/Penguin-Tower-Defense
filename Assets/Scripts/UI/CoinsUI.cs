using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    [SerializeField] private LocalizedString coinsLocalizedString;

    private void OnEnable()
    {
        coinsLocalizedString.StringChanged += UpdateText;

        EconomyManager.Instance.OnCoinsChanged += OnCoinsChanged;

        OnCoinsChanged(EconomyManager.Instance.Coins);
    }

    private void OnDisable()
    {
        coinsLocalizedString.StringChanged -= UpdateText;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int coins)
    {
        coinsLocalizedString.Arguments = new object[]
        {
            coins
        };

        coinsLocalizedString.RefreshString();
    }

    private void UpdateText(string value)
    {
        coinsText.text = value;
    }
}