using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MenuBlocker : MonoBehaviour,
    IPointerClickHandler
{
    private Action onClick;

    public void Initialize(Action callback)
    {
        onClick = callback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}