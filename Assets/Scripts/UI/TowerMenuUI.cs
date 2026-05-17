using UnityEngine;

public class TowerMenuUI : MonoBehaviour
{
    public static TowerMenuUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panel;
    [SerializeField] private MenuBlocker blocker;

    private void Awake()
    {
        Instance = this;

        blocker.Initialize(Close);

        Close();
    }

    public void Open(BuildNode node)
    {
        root.SetActive(true);

        panel.position =
            Camera.main.WorldToScreenPoint(
                node.transform.position);
    }

    public void Upgrade()
    {
        BuildManager.Instance.TryUpgrade();

        Close();
    }

    public void Sell()
    {
        BuildManager.Instance.TrySell();

        Close();
    }

    public void Close()
    {
        root.SetActive(false);
    }
}