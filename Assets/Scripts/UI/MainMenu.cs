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
        if (button == null)
            return;

        button.onClick.AddListener(action);

        if (AudioManager.Instance != null)
            button.onClick.AddListener(AudioManager.Instance.PlayClick);
    }

    private void UnbindButton(Button button, UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveListener(action);

        if (AudioManager.Instance != null)
            button.onClick.RemoveListener(AudioManager.Instance.PlayClick);
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
        if (LevelLoader.Instance != null && LevelLoader.Instance.CurrentLevel != null)
        {
            LevelLoader.Instance.ReturnToMap();
            CloseAllPanel();
            return;
        }

        ShowPanel(mainPanel);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void ShowPanel(GameObject panel)
    {
        CloseAllPanel();

        if (panel != null)
            panel.SetActive(true);
    }

    private void CloseAllPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
