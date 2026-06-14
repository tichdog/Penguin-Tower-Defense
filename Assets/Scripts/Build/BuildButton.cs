using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private BuildsBase buildData;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(Build);
    }

    private void Build()
    {
        if (BuildMenuUI.Instance != null)
            BuildMenuUI.Instance.Build(buildData);
    }
}
