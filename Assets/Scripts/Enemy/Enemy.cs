using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyData _data;
    private EnemyPath _path;

    [SerializeField] private int _currentPointIndex;
    [SerializeField] private float _currentHP;

    private bool _isInitialized;

    public bool IgnoreDefenders => _data.IgnoreDefenders;

    public float CurrentHP => _currentHP;

    public void Initialize(EnemyData data, EnemyPath path)
    {
        _data = data;

        _path = path;

        _currentHP = data.MaxHP;

        transform.position = _path.Points[0].position;

        _currentPointIndex = 1;

        _isInitialized = true;

        EnemyManager.Instance.RegisterEnemy(this);
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        Move();
    }

    private void Move()
    {
        if (_currentPointIndex >= _path.Points.Length)
        {
            ReachFinish();

            return;
        }

        Transform target = _path.Points[_currentPointIndex];

        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * _data.Speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= 0.1f)
        {
            _currentPointIndex++;
        }
    }

    private void ReachFinish()
    {
        // Нанести урон базе игрока

        Destroy(gameObject);
        EnemyManager.Instance.UnregisterEnemy(this);
    }

    public void TakeDamage(float damage)
    {
        damage -= _data.Armor;

        if (damage < 1f)
            damage = 1f;

        _currentHP -= damage;

        if (_currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        EconomyManager.Instance.AddCoins(_data.GetReward());
        EnemyManager.Instance.UnregisterEnemy(this);
        Destroy(gameObject);
    }
}