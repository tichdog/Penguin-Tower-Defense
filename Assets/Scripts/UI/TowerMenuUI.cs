using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenuUI : MonoBehaviour
{
    public static TowerMenuUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panel;
    [SerializeField] private MenuBlocker blocker;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text upgradeText;
    [SerializeField] private TMP_Text sellText;
    [SerializeField] private TMP_Text towerInfoText;

    [Header("Labels")]
    [SerializeField] private string upgradeFormat = "Улучшить\n-{0}";
    [SerializeField] private string upgradeNoMoneyFormat = "Не хватает\n-{0}";
    [SerializeField] private string maxLevelText = "Макс.\nуровень";
    [SerializeField] private string sellFormat = "Продать\n+{0}";
    [SerializeField] private string towerInfoFormat = "Ур. {0}\nУрон {1}-{2}\nРадиус {3}";

    private BuildNode currentNode;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        CacheReferences();
        EnsureInfoText();

        if (blocker != null)
            blocker.Initialize(Close);

        Close();
    }

    private void OnEnable()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged += OnCoinsChanged;
    }

    private void OnDisable()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
    }

    public void Open(BuildNode node)
    {
        currentNode = node;
        mainCamera = Camera.main;
        CacheReferences();
        EnsureInfoText();

        if (root == null || panel == null || mainCamera == null)
            return;

        root.SetActive(true);
        UpdatePanelPosition();
        Refresh();
    }

    private void LateUpdate()
    {
        if (root != null && root.activeSelf)
            UpdatePanelPosition();
    }

    public void Upgrade()
    {
        if (BuildManager.Instance == null)
            return;

        bool success = BuildManager.Instance.TryUpgrade();
        if (!success)
        {
            Refresh();
            return;
        }

        Refresh();
    }

    public void Sell()
    {
        if (BuildManager.Instance == null)
            return;

        bool success = BuildManager.Instance.TrySell();

        if (success)
            Close();
        else
            Refresh();
    }

    public void Close()
    {
        currentNode = null;

        if (root != null)
            root.SetActive(false);
    }

    private void Refresh()
    {
        Tower tower = currentNode != null ? currentNode.CurrentTower : null;

        if (tower == null)
        {
            Close();
            return;
        }

        RefreshInfo(tower);
        RefreshSell(tower);
        RefreshUpgrade(tower);
    }

    private void RefreshInfo(Tower tower)
    {
        if (towerInfoText == null || tower.Data == null)
            return;

        Vector2 damage = tower.Data.DamageRange;
        towerInfoText.text = string.Format(
            towerInfoFormat,
            tower.Data.Level,
            Mathf.RoundToInt(damage.x),
            Mathf.RoundToInt(damage.y),
            FormatFloat(tower.Data.AttackRadius)
        );
    }

    private void RefreshSell(Tower tower)
    {
        int sellPrice = tower.GetSellPrice();

        if (sellText != null)
            sellText.text = string.Format(sellFormat, sellPrice);

        if (sellButton != null)
            sellButton.interactable = sellPrice > 0;
    }

    private void RefreshUpgrade(Tower tower)
    {
        BuildsBase upgradeData = tower.GetUpgradeData();
        bool hasUpgrade = upgradeData != null && upgradeData.Prefab != null;
        int coins = EconomyManager.Instance != null ? EconomyManager.Instance.Coins : 0;
        bool canAfford = hasUpgrade && coins >= upgradeData.PurchasePrice;

        if (upgradeText != null)
        {
            if (!hasUpgrade)
                upgradeText.text = maxLevelText;
            else if (!canAfford)
                upgradeText.text = string.Format(upgradeNoMoneyFormat, upgradeData.PurchasePrice);
            else
                upgradeText.text = string.Format(upgradeFormat, upgradeData.PurchasePrice);
        }

        if (upgradeButton != null)
            upgradeButton.interactable = canAfford;
    }

    private void UpdatePanelPosition()
    {
        if (currentNode == null || panel == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        panel.position = mainCamera.WorldToScreenPoint(currentNode.transform.position);
    }

    private void OnCoinsChanged(int coins)
    {
        if (root != null && root.activeSelf)
            Refresh();
    }

    private void CacheReferences()
    {
        if (root == null)
            root = gameObject;

        if (panel == null)
            panel = GetComponentInChildren<RectTransform>(true);

        if (upgradeButton == null)
            upgradeButton = FindButton("UpgradeButton");

        if (sellButton == null)
            sellButton = FindButton("SellButton");

        if (upgradeText == null && upgradeButton != null)
            upgradeText = upgradeButton.GetComponentInChildren<TMP_Text>(true);

        if (sellText == null && sellButton != null)
            sellText = sellButton.GetComponentInChildren<TMP_Text>(true);

        PrepareButtonText(upgradeText);
        PrepareButtonText(sellText);
    }

    private Button FindButton(string objectName)
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
                return children[i].GetComponent<Button>();
        }

        return null;
    }

    private void PrepareButtonText(TMP_Text text)
    {
        if (text == null)
            return;

        text.enableAutoSizing = true;
        text.fontSizeMin = 11f;
        text.fontSizeMax = Mathf.Min(text.fontSizeMax, 22f);
        text.alignment = TextAlignmentOptions.Center;
    }

    private void EnsureInfoText()
    {
        if (towerInfoText != null || panel == null)
            return;

        GameObject textObject = new GameObject(
            "TowerInfoText",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );
        textObject.transform.SetParent(panel, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(190f, 105f);

        towerInfoText = textObject.GetComponent<TMP_Text>();
        towerInfoText.alignment = TextAlignmentOptions.Center;
        towerInfoText.fontSize = 18f;
        towerInfoText.enableAutoSizing = true;
        towerInfoText.fontSizeMin = 12f;
        towerInfoText.fontSizeMax = 22f;
        towerInfoText.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        towerInfoText.raycastTarget = false;
    }

    private string FormatFloat(float value)
    {
        return value.ToString("0.#");
    }
}
