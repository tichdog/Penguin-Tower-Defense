using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button exitButton;

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
        backButton.onClick.AddListener(OpenMainMenu);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(StartGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        backButton.onClick.RemoveListener(OpenMainMenu);
        exitButton.onClick.RemoveListener(ExitGame);
    }

    private void StartGame()
    {
        Debug.Log("GameStart");
        //SceneManager.LoadScene("Game");
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
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);

        panel.SetActive(true);
    }
}
