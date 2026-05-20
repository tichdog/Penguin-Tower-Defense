using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private LevelData[] levels;

    public LevelData[] Levels => levels;

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
}
