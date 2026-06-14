using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private readonly List<Tower> towers = new();

    private BuildNode selectedNode;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectNode(BuildNode node)
    {
        selectedNode = node;

        if (node.IsOccupied)
        {
            if (TowerMenuUI.Instance != null)
                TowerMenuUI.Instance.Open(node);
        }
        else
        {
            if (BuildMenuUI.Instance != null)
                BuildMenuUI.Instance.Open(node);
        }
    }

    public bool TryBuild(BuildsBase data)
    {
        if (selectedNode == null)
            return false;

        if (selectedNode.IsOccupied)
            return false;

        if (data == null || data.Prefab == null || EconomyManager.Instance == null)
            return false;

        if (!EconomyManager.Instance.TrySpend(data.PurchasePrice))
            return false;

        Tower tower = BuildFactory.Create(
            data,
            selectedNode
        );

        if (tower == null)
            return false;

        selectedNode.SetTower(tower);
        towers.Add(tower);

        return true;
    }

    public bool TrySell()
    {
        if (selectedNode == null)
            return false;

        if (!selectedNode.IsOccupied)
            return false;

        Tower tower = selectedNode.CurrentTower;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.AddCoins(
                tower.GetSellPrice()
            );

        towers.Remove(tower);
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

        if (upgradeData == null || upgradeData.Prefab == null)
            return false;

        if (EconomyManager.Instance == null)
            return false;

        if (!EconomyManager.Instance.TrySpend(
            upgradeData.PurchasePrice))
            return false;

        towers.Remove(currentTower);
        Destroy(currentTower.gameObject);

        Tower newTower = BuildFactory.Create(
            upgradeData,
            selectedNode
        );

        if (newTower == null)
            return false;

        selectedNode.SetTower(newTower);
        towers.Add(newTower);

        return true;
    }

    public void ClearAllTowers()
    {
        selectedNode = null;

        for (int i = towers.Count - 1; i >= 0; i--)
        {
            Tower tower = towers[i];

            if (tower != null)
                Destroy(tower.gameObject);
        }

        towers.Clear();

        if (BuildMenuUI.Instance != null)
            BuildMenuUI.Instance.Close();

        if (TowerMenuUI.Instance != null)
            TowerMenuUI.Instance.Close();
    }
}
