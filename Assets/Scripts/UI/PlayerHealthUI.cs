using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject healthRoot;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button victoryBackButton;
    [SerializeField] private TMP_Text victoryStarsText;
    [SerializeField] private string format = "\u0417\u0434\u043E\u0440\u043E\u0432\u044C\u0435: {0}/{1}";

    private bool isSubscribed;

    private void OnEnable()
    {
        EnsureDefeatPanel();
        SubscribeLevelLoader();
        Subscribe();
        BindBackButton();
        RefreshVisibility();
    }

    private void Start()
    {
        EnsureDefeatPanel();
        SubscribeLevelLoader();
        Subscribe();
        BindBackButton();
        RefreshVisibility();
    }

    private void OnDisable()
    {
        if (isSubscribed && PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.OnHealthChanged -= OnHealthChanged;
            PlayerHealthManager.Instance.OnPlayerDied -= OnPlayerDied;
        }

        UnbindBackButton();
        UnsubscribeLevelLoader();

        isSubscribed = false;
    }

    private void Subscribe()
    {
        if (isSubscribed)
            return;

        if (PlayerHealthManager.Instance == null)
            return;

        PlayerHealthManager.Instance.OnHealthChanged += OnHealthChanged;
        PlayerHealthManager.Instance.OnPlayerDied += OnPlayerDied;
        isSubscribed = true;

        OnHealthChanged(
            PlayerHealthManager.Instance.CurrentHealth,
            PlayerHealthManager.Instance.MaxHealth
        );
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (healthText == null)
            return;

        healthText.text = string.Format(format, currentHealth, maxHealth);
    }

    private void OnPlayerDied()
    {
        ShowResultPanel(defeatPanel);
    }

    private void OnLevelCompleted(LevelData levelData, int stars)
    {
        EnsureVictoryPanel();
        SetVictoryStars(stars);
        ShowResultPanel(victoryPanel);
    }

    private void OnLevelFailed(LevelData levelData)
    {
        ShowResultPanel(defeatPanel);
    }

    private void OnLevelLoaded(LevelData levelData)
    {
        RefreshVisibility();
    }

    private void OnLevelUnloaded()
    {
        RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        bool isLevelLoaded = LevelLoader.Instance != null && LevelLoader.Instance.CurrentLevel != null;

        if (healthRoot != null && healthRoot != gameObject)
            healthRoot.SetActive(isLevelLoaded);
        else if (healthText != null)
            healthText.enabled = isLevelLoaded;

        if (defeatPanel != null)
            defeatPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    private void SubscribeLevelLoader()
    {
        if (LevelLoader.Instance == null)
            return;

        LevelLoader.Instance.LevelLoaded -= OnLevelLoaded;
        LevelLoader.Instance.LevelUnloaded -= OnLevelUnloaded;
        LevelLoader.Instance.LevelCompleted -= OnLevelCompleted;
        LevelLoader.Instance.LevelFailed -= OnLevelFailed;
        LevelLoader.Instance.LevelLoaded += OnLevelLoaded;
        LevelLoader.Instance.LevelUnloaded += OnLevelUnloaded;
        LevelLoader.Instance.LevelCompleted += OnLevelCompleted;
        LevelLoader.Instance.LevelFailed += OnLevelFailed;
    }

    private void UnsubscribeLevelLoader()
    {
        if (LevelLoader.Instance == null)
            return;

        LevelLoader.Instance.LevelLoaded -= OnLevelLoaded;
        LevelLoader.Instance.LevelUnloaded -= OnLevelUnloaded;
        LevelLoader.Instance.LevelCompleted -= OnLevelCompleted;
        LevelLoader.Instance.LevelFailed -= OnLevelFailed;
    }

    private void BindBackButton()
    {
        BindButton(backButton);
        BindButton(victoryBackButton);
    }

    private void UnbindBackButton()
    {
        UnbindButton(backButton);
        UnbindButton(victoryBackButton);
    }

    private void ReturnToMap()
    {
        if (LevelLoader.Instance != null)
            LevelLoader.Instance.ReturnToMap();
    }

    private void EnsureDefeatPanel()
    {
        if (defeatPanel != null)
            return;

        defeatPanel = CreateResultPanel(
            "DefeatPanel",
            "\u0418\u0433\u0440\u043E\u043A \u043F\u0440\u043E\u0438\u0433\u0440\u0430\u043B",
            new Color(0f, 0f, 0f, 0.7f),
            out Button button
        );

        backButton = button;
    }

    private void EnsureVictoryPanel()
    {
        if (victoryPanel != null)
            return;

        victoryPanel = CreateResultPanel(
            "VictoryPanel",
            "\u041F\u043E\u0431\u0435\u0434\u0430!",
            new Color(0f, 0.32f, 0.18f, 0.76f),
            out Button button
        );

        victoryBackButton = button;
        victoryStarsText = CreateText(
            victoryPanel.transform,
            "\u0417\u0432\u0435\u0437\u0434\u044B: 0/3",
            new Vector2(0f, 5f),
            new Vector2(520f, 70f),
            38f,
            Color.white
        );
    }

    private GameObject CreateResultPanel(
        string panelName,
        string titleText,
        Color backgroundColor,
        out Button resultButton
    )
    {
        Transform parent = transform.parent != null ? transform.parent : transform;

        GameObject panel = new GameObject(
            panelName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = backgroundColor;
        panelImage.raycastTarget = true;

        TMP_Text title = CreateText(
            panel.transform,
            titleText,
            new Vector2(0f, 70f),
            new Vector2(700f, 120f),
            64f,
            Color.white
        );
        title.alignment = TextAlignmentOptions.Center;

        resultButton = CreateButton(panel.transform);
        BindButton(resultButton);

        panel.SetActive(false);
        return panel;
    }

    private TMP_Text CreateText(
        Transform parent,
        string value,
        Vector2 position,
        Vector2 size,
        float fontSize,
        Color color
    )
    {
        GameObject textObject = new GameObject(
            "Text",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;

        return text;
    }

    private Button CreateButton(Transform parent)
    {
        GameObject buttonObject = new GameObject(
            "BackToMapButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button)
        );
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, -65f);
        rectTransform.sizeDelta = new Vector2(260f, 70f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.95f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        TMP_Text label = CreateText(
            buttonObject.transform,
            "\u041D\u0430 \u043A\u0430\u0440\u0442\u0443",
            Vector2.zero,
            new Vector2(240f, 60f),
            32f,
            Color.black
        );
        label.raycastTarget = false;

        return button;
    }

    private void ShowResultPanel(GameObject panel)
    {
        EnsureDefeatPanel();

        if (panel == null)
            return;

        if (defeatPanel != null)
            defeatPanel.SetActive(panel == defeatPanel);

        if (victoryPanel != null)
            victoryPanel.SetActive(panel == victoryPanel);

        panel.transform.SetAsLastSibling();
    }

    private void BindButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveListener(ReturnToMap);
        button.onClick.AddListener(ReturnToMap);
    }

    private void UnbindButton(Button button)
    {
        if (button != null)
            button.onClick.RemoveListener(ReturnToMap);
    }

    private void SetVictoryStars(int stars)
    {
        if (victoryStarsText == null)
            return;

        victoryStarsText.text = $"\u0417\u0432\u0435\u0437\u0434\u044B: {Mathf.Clamp(stars, 0, 3)}/3";
    }
}
