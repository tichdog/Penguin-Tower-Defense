using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Gears")]
    [SerializeField] private CanvasGroup leftGear;
    [SerializeField] private CanvasGroup rightGear;

    [SerializeField] private GearRotate leftRotate;
    [SerializeField] private GearRotate rightRotate;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeRoutine;

    private void Start()
    {
        SetAlpha(0f);

        leftGear.gameObject.SetActive(false);
        rightGear.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        leftGear.gameObject.SetActive(true);
        rightGear.gameObject.SetActive(true);

        leftRotate.SetRotate(true);
        rightRotate.SetRotate(true);

        fadeRoutine = StartCoroutine(Fade(1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        leftRotate.SetRotate(false);
        rightRotate.SetRotate(false);

        fadeRoutine = StartCoroutine(Fade(0f, true));
    }

    private IEnumerator Fade(float target, bool disableAfter = false)
    {
        float start = leftGear.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;
            float alpha = Mathf.Lerp(start, target, t);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(target);

        if (disableAfter && target == 0f)
        {
            leftGear.gameObject.SetActive(false);
            rightGear.gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        leftGear.alpha = alpha;
        rightGear.alpha = alpha;
    }
}