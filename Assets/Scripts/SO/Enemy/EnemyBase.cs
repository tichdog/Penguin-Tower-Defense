using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private LocalizedString _enemyName;
    [SerializeField] private LocalizedString _description;

    [Header("Visual")]
    [SerializeField] private Sprite _icon;
    [SerializeField] private Enemy _prefab;

    [Header("Stats")]
    [SerializeField] private float _maxHP = 10f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private int _reward = 5;
    [SerializeField] private bool _ignoreDefenders;

    [Header("Defense")]
    [SerializeField] private float _armor = 0f;
    [SerializeField] private bool _isFlying = false;

    [Header("Spawn")]
    [SerializeField] private float _spawnScale = 1f;

    [Header("Reward")]
    [SerializeField] private int _minReward = 1;
    [SerializeField] private int _maxReward = 5;

    public LocalizedString EnemyName => _enemyName;
    public LocalizedString Description => _description;
    public Sprite Icon => _icon;
    public Enemy Prefab => _prefab;
    public float MaxHP => _maxHP;
    public float Speed => _speed;
    public float Damage => _damage;
    public int Reward => _reward;
    public float Armor => _armor;
    public bool IsFlying => _isFlying;
    public float SpawnScale => _spawnScale;
    public bool IgnoreDefenders => _ignoreDefenders;
    public int GetReward()
    {
        return Random.Range(_minReward, _maxReward + 1);
    }
}