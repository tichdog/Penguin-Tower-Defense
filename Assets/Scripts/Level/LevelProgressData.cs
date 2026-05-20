using System;
using System.Collections.Generic;

[Serializable]
public class LevelProgressData
{
    public List<LevelProgressEntry> levels = new List<LevelProgressEntry>();

    public void Normalize()
    {
        if (levels == null)
            levels = new List<LevelProgressEntry>();
    }
}

[Serializable]
public class LevelProgressEntry
{
    public string levelId = "";
    public int stars;
}
