using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private BuildsBase buildData;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private string priceFormat = "{0}\n-{1}";
    [SerializeField] private string noMoneyFormat = "{0}\nНужно {1}";

    private string baseLabel;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (label == null && button != null)
            label = button.GetComponentInChildren<TMP_Text>(true);

        if (label != null)
        {
            baseLabel = label.text;
            label.enableAutoSizing = true;
            label.fontSizeMin = 11f;
            label.fontSizeMax = Mathf.Min(label.fontSizeMax, 22f);
        }

        if (button != null)
            button.onClick.AddListener(Build);
    }

    private void OnEnable()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged += OnCoinsChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
    }

    private void Build()
    {
        if (BuildMenuUI.Instance != null)
        {
            BuildMenuUI.Instance.Build(buildData);
            Refresh();
        }
    }

    private void Refresh()
    {
        if (buildData == null)
        {
            if (button != null)
                button.interactable = false;

            return;
        }

        int coins = EconomyManager.Instance != null ? EconomyManager.Instance.Coins : 0;
        bool canAfford = coins >= buildData.PurchasePrice;

        if (button != null)
            button.interactable = canAfford;

        if (label != null)
        {
            string title = string.IsNullOrWhiteSpace(baseLabel) ? buildData.name : baseLabel;
            label.text = string.Format(
                canAfford ? priceFormat : noMoneyFormat,
                title,
                buildData.PurchasePrice
            );
        }
    }

    private void OnCoinsChanged(int coins)
    {
        Refresh();
    }
}
