using UnityEngine;

public class BuildNode : MonoBehaviour
{
    [SerializeField] private Transform buildPoint;

    public Tower CurrentTower { get; private set; }

    public bool IsOccupied => CurrentTower != null;

    public Vector3 BuildPosition => buildPoint.position;

    public void SetTower(Tower tower)
    {
        CurrentTower = tower;
    }

    public void Clear()
    {
        CurrentTower = null;
    }
}