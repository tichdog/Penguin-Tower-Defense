using UnityEngine;

[CreateAssetMenu(fileName = "BarracksTower", menuName = "Scriptable Objects/BarracksTower")]
public class BarracksTowerData : BuildsBase
{
    [Header("Spawn")]
    [SerializeField] private DefenderUnit _unitPrefab;
    [SerializeField] private int _maxActiveUnits = 3;
    [SerializeField] private float _spawnInterval = 4f;
    [SerializeField] private float _spawnRadius = 0.7f;
    [SerializeField] private bool _spawnOnlyWhenEnemiesInRange = true;

    [Header("Unit Stats")]
    [SerializeField] private float _unitMaxHealth = 12f;
    [SerializeField] private float _unitMoveSpeed = 2.2f;
    [SerializeField] private float _unitDamage = 3f;
    [SerializeField] private float _unitAttackCooldown = 1f;
    [SerializeField] private float _unitAttackRange = 0.45f;
    [SerializeField] private float _unitAggroRange = 3.5f;
    [SerializeField] private float _unitLifeTime;

    public DefenderUnit UnitPrefab => _unitPrefab;
    public int MaxActiveUnits => Mathf.Max(1, _maxActiveUnits);
    public float SpawnInterval => Mathf.Max(0.1f, _spawnInterval);
    public float SpawnRadius => Mathf.Max(0f, _spawnRadius);
    public bool SpawnOnlyWhenEnemiesInRange => _spawnOnlyWhenEnemiesInRange;
    public float UnitMaxHealth => Mathf.Max(1f, _unitMaxHealth);
    public float UnitMoveSpeed => Mathf.Max(0.1f, _unitMoveSpeed);
    public float UnitDamage => Mathf.Max(0f, _unitDamage);
    public float UnitAttackCooldown => Mathf.Max(0.1f, _unitAttackCooldown);
    public float UnitAttackRange => Mathf.Max(0.05f, _unitAttackRange);
    public float UnitAggroRange => Mathf.Max(0.1f, _unitAggroRange);
    public float UnitLifeTime => Mathf.Max(0f, _unitLifeTime);
}
