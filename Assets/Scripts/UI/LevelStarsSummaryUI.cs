using TMPro;
using SaveSystem;
using UnityEngine;

public class LevelStarsSummaryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private LevelSelectController levelSelectController;
    [SerializeField] private bool countOnlyUnlockedLevels = true;

    private void Awake()
    {
        if (starsText == null)
            starsText = GetComponentInChildren<TMP_Text>();

        if (levelSelectController == null)
            levelSelectController = FindAnyObjectByType<LevelSelectController>();
    }

    private void OnEnable()
    {
        GameDataManager.OnLoaded += OnSaveLoaded;
        LevelProgressService.StarsChanged += OnStarsChanged;

        Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        GameDataManager.OnLoaded -= OnSaveLoaded;
        LevelProgressService.StarsChanged -= OnStarsChanged;
    }

    public void Refresh()
    {
        if (starsText == null || levelSelectController == null)
            return;

        LevelData[] levels = levelSelectController.Levels;
        if (levels == null)
        {
            starsText.text = "0/0";
            return;
        }

        int earnedStars = 0;
        int availableStars = 0;

        for (int i = 0; i < levels.Length; i++)
        {
            LevelData level = levels[i];
            if (level == null)
                continue;

            bool isUnlocked = levelSelectController.IsUnlocked(i);
            if (!countOnlyUnlockedLevels || isUnlocked)
                availableStars += 3;

            earnedStars += LevelProgressService.GetStars(level.LevelId);
        }

        starsText.text = $"{earnedStars}/{availableStars}";
    }

    private void OnSaveLoaded(int slot)
    {
        Refresh();
    }

    private void OnStarsChanged(string levelId, int stars)
    {
        Refresh();
    }
}
