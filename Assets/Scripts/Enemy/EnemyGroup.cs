using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    [SerializeField] private EnemyData _enemy;

    [SerializeField] private int _count = 5;

    [SerializeField] private float _delayBetweenEnemies = 1f;

    public EnemyData Enemy => _enemy;

    public int Count => _count;

    public float DelayBetweenEnemies => _delayBetweenEnemies;
}