using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public BuildsBase Data { get; private set; }

    public BuildNode Node { get; private set; }

    public virtual void Initialize(
        BuildsBase data,
        BuildNode node)
    {
        Data = data;
        Node = node;
    }

    public bool CanUpgrade()
    {
        return Data.HasUpgrade;
    }

    public BuildsBase GetUpgradeData()
    {
        return Data.NextLevel;
    }

    public int GetSellPrice()
    {
        return Data.SalePrice;
    }
}
