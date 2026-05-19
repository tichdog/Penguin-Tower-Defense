using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WaveConfig _waveConfig;

    [SerializeField] private EnemyPath _path;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        Wave[] waves = _waveConfig.Waves;

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            Wave currentWave = waves[waveIndex];

            yield return StartCoroutine(SpawnWave(currentWave));

            yield return new WaitForSeconds(currentWave.DelayAfterWave);
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        EnemyGroup[] groups = wave.Groups;

        for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
        {
            EnemyGroup currentGroup = groups[groupIndex];

            for (int i = 0; i < currentGroup.Count; i++)
            {
                SpawnEnemy(currentGroup.Enemy);

                yield return new WaitForSeconds(currentGroup.DelayBetweenEnemies);
            }
        }
    }

    private void SpawnEnemy(EnemyData data)
    {
        Enemy enemy = Instantiate(data.Prefab);

        enemy.Initialize(data, _path);
    }
}