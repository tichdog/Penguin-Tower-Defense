using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backSettingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backMenuButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        OpenMainMenu();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        backSettingsButton.onClick.AddListener(OpenMainMenu);
        exitButton.onClick.AddListener(ExitGame);
        backMenuButton.onClick.AddListener(OpenMainMenu);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(StartGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        backSettingsButton.onClick.RemoveListener(OpenMainMenu);
        exitButton.onClick.RemoveListener(ExitGame);
        backMenuButton.onClick.AddListener(OpenMainMenu);
    }

    private void StartGame()
    {
        CloseAllPanel();
    }

    private void OpenSettings()
    {
        ShowPanel(settingsPanel);
    }

    private void OpenMainMenu()
    {
        ShowPanel(mainPanel);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void ShowPanel(GameObject panel)
    {
        CloseAllPanel();
        panel.SetActive(true);
    }

    private void CloseAllPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}
