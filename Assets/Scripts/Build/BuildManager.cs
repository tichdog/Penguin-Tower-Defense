using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private BuildNode selectedNode;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectNode(BuildNode node)
    {
        selectedNode = node;

        if (node.IsOccupied)
            TowerMenuUI.Instance.Open(node);
        else
            BuildMenuUI.Instance.Open(node);
    }

    public bool TryBuild(BuildsBase data)
    {
        if (selectedNode == null)
            return false;

        if (selectedNode.IsOccupied)
            return false;

        if (!EconomyManager.Instance.TrySpend(data.PurchasePrice))
            return false;

        Tower tower = BuildFactory.Create(
            data,
            selectedNode
        );

        selectedNode.SetTower(tower);

        return true;
    }

    public bool TrySell()
    {
        if (selectedNode == null)
            return false;

        if (!selectedNode.IsOccupied)
            return false;

        Tower tower = selectedNode.CurrentTower;

        EconomyManager.Instance.AddCoins(
            tower.GetSellPrice()
        );

        Destroy(tower.gameObject);

        selectedNode.Clear();

        return true;
    }

    public bool TryUpgrade()
    {
        if (selectedNode == null)
            return false;

        if (!selectedNode.IsOccupied)
            return false;

        Tower currentTower = selectedNode.CurrentTower;

        if (!currentTower.CanUpgrade())
            return false;

        BuildsBase upgradeData =
            currentTower.GetUpgradeData();

        if (!EconomyManager.Instance.TrySpend(
            upgradeData.PurchasePrice))
            return false;

        Destroy(currentTower.gameObject);

        Tower newTower = BuildFactory.Create(
            upgradeData,
            selectedNode
        );

        selectedNode.SetTower(newTower);

        return true;
    }
}