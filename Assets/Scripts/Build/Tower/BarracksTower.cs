using System.Collections.Generic;
using UnityEngine;

public class BarracksTower : Tower
{
    private readonly List<DefenderUnit> activeUnits = new();

    private float spawnTimer;

    public override void Initialize(BuildsBase data, BuildNode node)
    {
        base.Initialize(data, node);

        BarracksTowerData barracksData = Data as BarracksTowerData;
        spawnTimer = barracksData != null ? barracksData.SpawnInterval : 0f;
    }

    protected override void Tick()
    {
        BarracksTowerData barracksData = Data as BarracksTowerData;
        if (barracksData == null)
            return;

        CleanupActiveUnits();

        if (activeUnits.Count >= barracksData.MaxActiveUnits)
            return;

        if (barracksData.SpawnOnlyWhenEnemiesInRange && !HasEnemyInRange())
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < barracksData.SpawnInterval)
            return;

        spawnTimer = 0f;
        SpawnUnit(barracksData);
    }

    private void SpawnUnit(BarracksTowerData barracksData)
    {
        Vector2 offset = Random.insideUnitCircle * barracksData.SpawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(offset.x, offset.y, 0f);
        spawnPosition.z = transform.position.z;

        DefenderUnit unit = barracksData.UnitPrefab != null
            ? Instantiate(barracksData.UnitPrefab, spawnPosition, Quaternion.identity)
            : DefenderUnit.CreateDefault(spawnPosition);

        if (unit == null)
            return;

        unit.Initialize(barracksData, transform.position);
        unit.Removed += OnUnitRemoved;
        activeUnits.Add(unit);
    }

    private bool HasEnemyInRange()
    {
        if (EnemyManager.Instance == null)
            return false;

        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (enemy == null || enemy.IgnoreDefenders)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= Data.AttackRadius)
                return true;
        }

        return false;
    }

    private void CleanupActiveUnits()
    {
        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {
            if (activeUnits[i] != null)
                continue;

            activeUnits.RemoveAt(i);
        }
    }

    private void OnUnitRemoved(DefenderUnit unit)
    {
        if (unit != null)
            unit.Removed -= OnUnitRemoved;

        activeUnits.Remove(unit);
    }

    private void OnDestroy()
    {
        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {
            DefenderUnit unit = activeUnits[i];
            if (unit == null)
                continue;

            unit.Removed -= OnUnitRemoved;
            Destroy(unit.gameObject);
        }

        activeUnits.Clear();
    }
}
