using System;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform levelParent;
    [SerializeField] private MapCameraController mapCameraController;
    [SerializeField] private SpriteRenderer mapRenderer;

    [Header("HUD")]
    [SerializeField] private GameObject[] mapOnlyObjects;
    [SerializeField] private GameObject[] levelOnlyObjects;

    public event Action<LevelData> LevelLoaded;
    public event Action LevelUnloaded;
    public event Action<LevelData, int> LevelCompleted;
    public event Action<LevelData> LevelFailed;

    private GameObject currentLevelInstance;
    private LevelData currentLevel;
    private EnemySpawner currentSpawner;
    private bool allWavesSpawned;
    private bool levelFinished;

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

    private void Start()
    {
        SetHudVisibility(currentLevel != null);
    }

    public void LoadLevel(LevelData levelData)
    {
        if (levelData == null || levelData.Prefab == null)
        {
            Debug.LogWarning("[LevelLoader] Level data or prefab is empty.");
            return;
        }

        UnloadCurrentLevel();

        levelFinished = false;
        allWavesSpawned = false;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.ResetCoins();

        if (PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.ResetHealth();
            PlayerHealthManager.Instance.OnPlayerDied -= FailCurrentLevel;
            PlayerHealthManager.Instance.OnPlayerDied += FailCurrentLevel;
        }

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

        currentSpawner = currentLevelInstance.GetComponentInChildren<EnemySpawner>();
        if (currentSpawner != null)
            currentSpawner.AllWavesSpawned += OnAllWavesSpawned;
        else
            Debug.LogWarning("[LevelLoader] Level prefab needs EnemySpawner for victory detection.");

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCountChanged -= OnEnemyCountChanged;
            EnemyManager.Instance.OnEnemyCountChanged += OnEnemyCountChanged;
        }

        SetMapVisible(false);
        SetHudVisibility(true);

        LevelLoaded?.Invoke(levelData);
    }

    public void UnloadCurrentLevel()
    {
        UnsubscribeGameSignals();
        levelFinished = true;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ClearAllEnemies();

        if (BuildManager.Instance != null)
            BuildManager.Instance.ClearAllTowers();

        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);

        currentLevelInstance = null;
        currentLevel = null;

        SetMapVisible(true);
        SetHudVisibility(false);

        if (mapRenderer != null && mapCameraController != null)
            mapCameraController.SetMapRenderer(mapRenderer);

        LevelUnloaded?.Invoke();
    }

    public void ReturnToMap()
    {
        UnloadCurrentLevel();
    }

    public void CompleteCurrentLevel(int stars)
    {
        if (currentLevel == null)
            return;

        if (levelFinished)
            return;

        levelFinished = true;
        StopSpawner();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ClearAllEnemies();

        stars = Mathf.Clamp(stars, 1, 3);
        LevelProgressService.SetStars(currentLevel.LevelId, stars);
        LevelCompleted?.Invoke(currentLevel, stars);
    }

    private void SetMapVisible(bool isVisible)
    {
        if (mapRenderer != null)
            mapRenderer.gameObject.SetActive(isVisible);
    }

    private void SetHudVisibility(bool isLevelLoaded)
    {
        SetObjectsActive(mapOnlyObjects, !isLevelLoaded);
        SetObjectsActive(levelOnlyObjects, isLevelLoaded);
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        if (objects == null)
            return;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(isActive);
        }
    }

    private void OnAllWavesSpawned()
    {
        allWavesSpawned = true;
        TryCompleteByGameState();
    }

    private void OnEnemyCountChanged(int aliveCount)
    {
        TryCompleteByGameState();
    }

    private void TryCompleteByGameState()
    {
        if (levelFinished || currentLevel == null || !allWavesSpawned)
            return;

        int aliveCount = EnemyManager.Instance != null ? EnemyManager.Instance.AliveCount : 0;
        if (aliveCount > 0)
            return;

        CompleteCurrentLevel(CalculateStars());
    }

    private int CalculateStars()
    {
        if (PlayerHealthManager.Instance == null)
            return 1;

        float healthPercent = PlayerHealthManager.Instance.MaxHealth > 0
            ? (float)PlayerHealthManager.Instance.CurrentHealth / PlayerHealthManager.Instance.MaxHealth
            : 0f;

        if (healthPercent >= 0.75f)
            return 3;

        if (healthPercent >= 0.4f)
            return 2;

        return 1;
    }

    private void FailCurrentLevel()
    {
        if (levelFinished || currentLevel == null)
            return;

        levelFinished = true;
        StopSpawner();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ClearAllEnemies();

        LevelFailed?.Invoke(currentLevel);
    }

    private void StopSpawner()
    {
        if (currentSpawner != null)
            currentSpawner.StopSpawning();
    }

    private void UnsubscribeGameSignals()
    {
        StopSpawner();

        if (currentSpawner != null)
            currentSpawner.AllWavesSpawned -= OnAllWavesSpawned;

        currentSpawner = null;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.OnEnemyCountChanged -= OnEnemyCountChanged;

        if (PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.OnPlayerDied -= FailCurrentLevel;
    }
}
