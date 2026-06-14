using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelMapPoint : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private LevelSelectController levelSelectController;
    [SerializeField] private int levelIndex;

    [Header("View")]
    [SerializeField] private SpriteRenderer pointRenderer;
    [SerializeField] private Sprite activeStarRenderer;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(1f, 1f, 1f, 0.45f);
    [SerializeField] private GameObject[] starsObjects;

    private SpriteRenderer[] starRenderers;
    private Sprite[] inactiveStarSprites;
    private Color[] inactiveStarColors;

    private void Awake()
    {
        if (pointRenderer == null)
            pointRenderer = GetComponent<SpriteRenderer>();

        CacheStars();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnMouseDown()
    {
        if (levelSelectController == null)
            return;

        levelSelectController.TryLoadLevel(levelIndex);
    }

    public void Refresh()
    {
        if (levelSelectController == null)
            return;

        CacheStars();

        LevelData levelData = levelSelectController.GetLevel(levelIndex);
        bool unlocked = levelSelectController.IsUnlocked(levelIndex);
        int stars = levelData != null ? LevelProgressService.GetStars(levelData.LevelId) : 0;

        if (pointRenderer != null)
            pointRenderer.color = unlocked ? unlockedColor : lockedColor;

        for (int i = 0; i < starRenderers.Length; i++)
        {
            SpriteRenderer starRenderer = starRenderers[i];
            if (starRenderer == null)
                continue;

            bool isActiveStar = unlocked && i < stars;
            starRenderer.sprite = isActiveStar && activeStarRenderer != null
                ? activeStarRenderer
                : inactiveStarSprites[i];

            starRenderer.color = isActiveStar
                ? Color.white
                : inactiveStarColors[i];
        }
    }

    private void CacheStars()
    {
        if (starsObjects == null)
        {
            starRenderers = new SpriteRenderer[0];
            inactiveStarSprites = new Sprite[0];
            inactiveStarColors = new Color[0];
            return;
        }

        if (starRenderers != null && starRenderers.Length == starsObjects.Length)
            return;

        starRenderers = new SpriteRenderer[starsObjects.Length];
        inactiveStarSprites = new Sprite[starsObjects.Length];
        inactiveStarColors = new Color[starsObjects.Length];

        for (int i = 0; i < starsObjects.Length; i++)
        {
            if (starsObjects[i] == null)
                continue;

            starRenderers[i] = starsObjects[i].GetComponent<SpriteRenderer>();

            if (starRenderers[i] == null)
                continue;

            inactiveStarSprites[i] = starRenderers[i].sprite;
            inactiveStarColors[i] = starRenderers[i].color;
        }
    }
}
