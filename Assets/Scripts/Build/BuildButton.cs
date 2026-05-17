using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private BuildsBase buildData;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(Build);
    }

    private void Build()
    {
        BuildMenuUI.Instance.Build(buildData);
    }
}