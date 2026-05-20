using UnityEngine;

public class LevelRoot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;

    public SpriteRenderer BackgroundRenderer => backgroundRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponentInChildren<SpriteRenderer>();
    }
#endif
}
