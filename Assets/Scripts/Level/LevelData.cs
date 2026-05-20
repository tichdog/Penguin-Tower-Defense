using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private string levelId;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject prefab;

    public string LevelId => levelId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? levelId : displayName;
    public GameObject Prefab => prefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(levelId))
            levelId = name;
    }
#endif
}
