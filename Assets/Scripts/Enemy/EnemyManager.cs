using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private readonly List<Enemy> _enemies = new();

    public List<Enemy> Enemies => _enemies;
    public int AliveCount => _enemies.Count;

    public event Action<int> OnEnemyCountChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
            NotifyEnemyCountChanged();
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
            NotifyEnemyCountChanged();
        }
    }

    public void ClearAllEnemies()
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _enemies[i];

            if (enemy != null)
                Destroy(enemy.gameObject);
        }

        _enemies.Clear();
        NotifyEnemyCountChanged();
    }

    private void NotifyEnemyCountChanged()
    {
        OnEnemyCountChanged?.Invoke(_enemies.Count);
    }
}
