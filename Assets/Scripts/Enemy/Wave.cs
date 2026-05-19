using UnityEngine;

[System.Serializable]
public class Wave
{
    [SerializeField] private EnemyGroup[] _groups;

    [SerializeField] private float _delayAfterWave = 5f;

    public EnemyGroup[] Groups => _groups;

    public float DelayAfterWave => _delayAfterWave;
}