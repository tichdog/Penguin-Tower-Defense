using UnityEngine;

public static class BuildFactory
{
    public static Tower Create(
        BuildsBase data,
        BuildNode node)
    {
        if (data == null || data.Prefab == null || node == null)
            return null;

        Tower tower = Object.Instantiate(
            data.Prefab,
            node.BuildPosition,
            Quaternion.identity
        );

        tower.Initialize(data, node);

        return tower;
    }
}
