using UnityEngine;
using UnityEngine.Events;
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
        BindButton(playButton, StartGame);
        BindButton(settingsButton, OpenSettings);
        BindButton(backSettingsButton, OpenMainMenu);
        BindButton(exitButton, ExitGame);
        BindButton(backMenuButton, OpenMainMenu);
    }

    private void OnDisable()
    {
        UnbindButton(playButton, StartGame);
        UnbindButton(settingsButton, OpenSettings);
        UnbindButton(backSettingsButton, OpenMainMenu);
        UnbindButton(exitButton, ExitGame);
        UnbindButton(backMenuButton, OpenMainMenu);
    }

    private void BindButton(Button button, UnityAction action)
    {
        button.onClick.AddListener(action);
        button.onClick.AddListener(AudioManager.Instance.PlayClick);
    }

    private void UnbindButton(Button button, UnityAction action)
    {
        button.onClick.RemoveListener(action);
        button.onClick.RemoveListener(AudioManager.Instance.PlayClick);
    }


    private void StartGame()
    {
        CloseAllPanel();
    }

    private void OpenSettings()
    {
        settingsPanel.SetActive(true);
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
