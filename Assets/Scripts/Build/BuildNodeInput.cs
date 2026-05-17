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
        EconomyManager.Instance.AddCoins(25);
        BuildManager.Instance.SelectNode(node);
    }
}