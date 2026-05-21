using TMPro;
using Unity.VisualScripting;
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

    private void Awake()
    {
        if (pointRenderer == null)
            pointRenderer = GetComponent<SpriteRenderer>();
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

        LevelData levelData = levelSelectController.GetLevel(levelIndex);
        bool unlocked = levelSelectController.IsUnlocked(levelIndex);
        int stars = levelData != null ? LevelProgressService.GetStars(levelData.LevelId) : 0;

        if (pointRenderer != null)
            pointRenderer.color = unlocked ? unlockedColor : lockedColor;

        for (int i = 1; i <= stars; i++)
        {
            starsObjects[i].GetComponent<SpriteRenderer>().sprite = activeStarRenderer;
        }

        Debug.Log($"Level {levelIndex} - Unlocked: {unlocked}, Stars: {stars}");
    }
}
