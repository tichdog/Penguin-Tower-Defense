using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BuildNode))]
public class BuildNodeInput : MonoBehaviour,
    IPointerClickHandler
{
    private BuildNode node;

    private void Awake()
    {
        node = GetComponent<BuildNode>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (BuildManager.Instance != null)
            BuildManager.Instance.SelectNode(node);
    }
}
