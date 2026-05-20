using System;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform levelParent;
    [SerializeField] private MapCameraController mapCameraController;

    [Header("UI")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject gameplayPanel;

    public event Action<LevelData> LevelLoaded;

    private GameObject currentLevelInstance;
    private LevelData currentLevel;

    public LevelData CurrentLevel => currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadLevel(LevelData levelData)
    {
        if (levelData == null || levelData.Prefab == null)
        {
            Debug.LogWarning("[LevelLoader] Level data or prefab is empty.");
            return;
        }

        UnloadCurrentLevel();

        Transform parent = levelParent != null ? levelParent : transform;
        currentLevelInstance = Instantiate(levelData.Prefab, parent);
        currentLevel = levelData;

        LevelRoot levelRoot = currentLevelInstance.GetComponent<LevelRoot>();
        if (levelRoot == null)
            levelRoot = currentLevelInstance.GetComponentInChildren<LevelRoot>();

        if (levelRoot != null && levelRoot.BackgroundRenderer != null && mapCameraController != null)
            mapCameraController.SetMapRenderer(levelRoot.BackgroundRenderer);
        else
            Debug.LogWarning("[LevelLoader] Level prefab needs LevelRoot with BackgroundRenderer for camera bounds.");

        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        LevelLoaded?.Invoke(levelData);
    }

    public void UnloadCurrentLevel()
    {
        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);

        currentLevelInstance = null;
        currentLevel = null;
    }

    public void CompleteCurrentLevel(int stars)
    {
        if (currentLevel == null)
            return;

        LevelProgressService.SetStars(currentLevel.LevelId, stars);
    }
}
