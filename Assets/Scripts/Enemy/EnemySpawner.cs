using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    private class SpawnRoute
    {
        [SerializeField] private EnemyPath enemyPath;
        [SerializeField] private WaveConfig waveConfig;

        public EnemyPath EnemyPath => enemyPath;
        public WaveConfig WaveConfig => waveConfig;
        public bool IsValid => enemyPath != null && waveConfig != null;
    }

    [Header("Routes")]
    [SerializeField] private SpawnRoute[] _routes;

    private readonly List<Coroutine> spawnCoroutines = new();
    private bool isSpawning;
    private int activeRoutes;

    public event Action AllWavesSpawned;

    private void Start()
    {
        Begin();
    }

    public void Begin()
    {
        if (isSpawning)
            return;

        if (_routes == null || _routes.Length == 0)
        {
            FinishSpawning("[EnemySpawner] Routes are missing.");
            return;
        }

        List<SpawnRoute> validRoutes = new();
        spawnCoroutines.Clear();

        for (int i = 0; i < _routes.Length; i++)
        {
            SpawnRoute route = _routes[i];
            if (route == null || !route.IsValid)
            {
                Debug.LogWarning($"[EnemySpawner] Route {i} needs EnemyPath and WaveConfig.");
                continue;
            }

            validRoutes.Add(route);
        }

        if (validRoutes.Count == 0)
        {
            FinishSpawning("[EnemySpawner] No valid routes to spawn.");
            return;
        }

        isSpawning = true;
        activeRoutes = validRoutes.Count;

        for (int i = 0; i < validRoutes.Count; i++)
            spawnCoroutines.Add(StartCoroutine(SpawnRouteRoutine(validRoutes[i])));
    }

    public void StopSpawning()
    {
        for (int i = 0; i < spawnCoroutines.Count; i++)
        {
            if (spawnCoroutines[i] != null)
                StopCoroutine(spawnCoroutines[i]);
        }

        spawnCoroutines.Clear();
        activeRoutes = 0;
        isSpawning = false;
    }

    private IEnumerator SpawnRouteRoutine(SpawnRoute route)
    {
        Wave[] waves = route.WaveConfig.Waves;

        if (waves == null || waves.Length == 0)
        {
            CompleteRoute();
            yield break;
        }

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            Wave currentWave = waves[waveIndex];

            if (currentWave == null)
                continue;

            yield return StartCoroutine(SpawnWave(currentWave, route.EnemyPath));

            yield return new WaitForSeconds(currentWave.DelayAfterWave);
        }

        CompleteRoute();
    }

    private IEnumerator SpawnWave(Wave wave, EnemyPath path)
    {
        EnemyGroup[] groups = wave.Groups;

        if (groups == null)
            yield break;

        for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
        {
            EnemyGroup currentGroup = groups[groupIndex];

            if (currentGroup == null || currentGroup.Enemy == null)
                continue;

            for (int i = 0; i < currentGroup.Count; i++)
            {
                SpawnEnemy(currentGroup.Enemy, path);

                yield return new WaitForSeconds(currentGroup.DelayBetweenEnemies);
            }
        }
    }

    private void SpawnEnemy(EnemyData data, EnemyPath path)
    {
        if (data == null || data.Prefab == null || path == null)
            return;

        Enemy enemy = Instantiate(data.Prefab);
        enemy.Initialize(data, path);
    }

    private void CompleteRoute()
    {
        activeRoutes = Mathf.Max(0, activeRoutes - 1);

        if (activeRoutes == 0)
            FinishSpawning();
    }

    private void FinishSpawning(string warning = null)
    {
        if (!string.IsNullOrWhiteSpace(warning))
            Debug.LogWarning(warning);

        isSpawning = false;
        spawnCoroutines.Clear();
        AllWavesSpawned?.Invoke();
    }
}
