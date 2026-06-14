using SaveSystem;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private LevelData[] levels;
    [SerializeField] private LevelMapPoint[] mapPoints;
    [SerializeField] private LevelStarsSummaryUI starsSummaryUI;

    public LevelData[] Levels => levels;

    private bool isSubscribedToLevelLoader;

    private void Awake()
    {
        CacheMapPoints();
    }

    private void OnEnable()
    {
        GameDataManager.OnLoaded += OnSaveLoaded;
        LevelProgressService.StarsChanged += OnStarsChanged;

        TrySubscribeLevelLoader();

        RefreshAllPoints();
    }

    private void Start()
    {
        TrySubscribeLevelLoader();
        RefreshAllPoints();
    }

    private void OnDisable()
    {
        GameDataManager.OnLoaded -= OnSaveLoaded;
        LevelProgressService.StarsChanged -= OnStarsChanged;

        if (isSubscribedToLevelLoader && LevelLoader.Instance != null)
            LevelLoader.Instance.LevelUnloaded -= RefreshAllPoints;

        isSubscribedToLevelLoader = false;
    }

    public bool IsUnlocked(int levelIndex)
    {
        return LevelProgressService.IsUnlocked(levels, levelIndex);
    }

    public LevelData GetLevel(int levelIndex)
    {
        if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
            return null;

        return levels[levelIndex];
    }

    public void TryLoadLevel(int levelIndex)
    {
        TryLoadLevel(GetLevel(levelIndex), levelIndex);
    }

    public void TryLoadLevel(LevelData levelData, int levelIndex)
    {
        if (!IsUnlocked(levelIndex))
            return;

        if (LevelLoader.Instance != null)
            LevelLoader.Instance.LoadLevel(levelData);
    }

    public void RefreshAllPoints()
    {
        CacheMapPoints();

        for (int i = 0; i < mapPoints.Length; i++)
        {
            if (mapPoints[i] != null)
                mapPoints[i].Refresh();
        }

        if (starsSummaryUI != null)
            starsSummaryUI.Refresh();
    }

    private void CacheMapPoints()
    {
        if (mapPoints != null && mapPoints.Length > 0)
            return;

        mapPoints = GetComponentsInChildren<LevelMapPoint>(true);
    }

    private void OnSaveLoaded(int slot)
    {
        RefreshAllPoints();
    }

    private void OnStarsChanged(string levelId, int stars)
    {
        RefreshAllPoints();
    }

    private void TrySubscribeLevelLoader()
    {
        if (isSubscribedToLevelLoader || LevelLoader.Instance == null)
            return;

        LevelLoader.Instance.LevelUnloaded += RefreshAllPoints;
        isSubscribedToLevelLoader = true;
    }
}
