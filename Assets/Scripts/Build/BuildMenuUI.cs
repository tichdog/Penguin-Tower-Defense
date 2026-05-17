using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panel;
    [SerializeField] private MenuBlocker blocker;

    private BuildNode currentNode;

    private void Awake()
    {
        Instance = this;

        blocker.Initialize(Close);

        Close();
    }

    public void Open(BuildNode node)
    {
        currentNode = node;

        root.SetActive(true);

        panel.position =
            Camera.main.WorldToScreenPoint(
                node.transform.position);
    }

    public void Build(BuildsBase build)
    {
        bool success =
            BuildManager.Instance.TryBuild(build);

        if (success)
            Close();
    }

    public void Close()
    {
        currentNode = null;

        root.SetActive(false);
    }
}