using UnityEngine;

public class TowerMenuUI : MonoBehaviour
{
    public static TowerMenuUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panel;
    [SerializeField] private MenuBlocker blocker;

    private BuildNode currentNode;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;

        if (blocker != null)
            blocker.Initialize(Close);

        Close();
    }

    public void Open(BuildNode node)
    {
        currentNode = node;
        mainCamera = Camera.main;

        if (root == null || panel == null || mainCamera == null)
            return;

        root.SetActive(true);
        UpdatePanelPosition();
    }

    private void LateUpdate()
    {
        if (root != null && root.activeSelf)
            UpdatePanelPosition();
    }

    public void Upgrade()
    {
        if (BuildManager.Instance != null)
            BuildManager.Instance.TryUpgrade();

        Close();
    }

    public void Sell()
    {
        if (BuildManager.Instance != null)
            BuildManager.Instance.TrySell();

        Close();
    }

    public void Close()
    {
        currentNode = null;

        if (root != null)
            root.SetActive(false);
    }

    private void UpdatePanelPosition()
    {
        if (currentNode == null || panel == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        panel.position = mainCamera.WorldToScreenPoint(currentNode.transform.position);
    }
}
