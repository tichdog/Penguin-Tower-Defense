using SaveSystem;
using UnityEngine;

public static class LevelProgressService
{
    public static LevelProgressData Load()
    {
        LevelProgressData progress = GameDataManager.Instance != null
            ? GameDataManager.Instance.GetData(SaveKeys.LevelsProgress, new LevelProgressData())
            : new LevelProgressData();

        progress.Normalize();
        return progress;
    }

    public static int GetStars(string levelId)
    {
        if (string.IsNullOrWhiteSpace(levelId))
            return 0;

        LevelProgressData progress = Load();
        LevelProgressEntry entry = Find(progress, levelId);
        return entry != null ? Mathf.Clamp(entry.stars, 0, 3) : 0;
    }

    public static void SetStars(string levelId, int stars, bool saveImmediately = true)
    {
        if (string.IsNullOrWhiteSpace(levelId) || GameDataManager.Instance == null)
            return;

        LevelProgressData progress = Load();
        LevelProgressEntry entry = Find(progress, levelId);

        if (entry == null)
        {
            entry = new LevelProgressEntry { levelId = levelId };
            progress.levels.Add(entry);
        }

        entry.stars = Mathf.Max(entry.stars, Mathf.Clamp(stars, 0, 3));

        GameDataManager.Instance.SetData(SaveKeys.LevelsProgress, progress);

        if (saveImmediately)
            GameDataManager.Instance.Save();
    }

    public static bool IsUnlocked(LevelData[] levels, int levelIndex)
    {
        if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
            return false;

        if (levelIndex == 0)
            return true;

        LevelData previousLevel = levels[levelIndex - 1];
        return previousLevel != null && GetStars(previousLevel.LevelId) >= 1;
    }

    private static LevelProgressEntry Find(LevelProgressData progress, string levelId)
    {
        progress.Normalize();

        for (int i = 0; i < progress.levels.Count; i++)
        {
            LevelProgressEntry entry = progress.levels[i];
            if (entry != null && entry.levelId == levelId)
                return entry;
        }

        return null;
    }
}
