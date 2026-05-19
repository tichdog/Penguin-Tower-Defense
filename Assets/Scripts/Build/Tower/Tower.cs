using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public BuildsBase Data { get; private set; }

    public BuildNode Node { get; private set; }

    private Enemy _target;

    private float _attackTimer;

    public virtual void Initialize(
        BuildsBase data,
        BuildNode node)
    {
        Data = data;
        Node = node;
    }

    private void Update()
    {
        FindTarget();

        Attack();
    }

    private void FindTarget()
    {
        float closestDistance = Mathf.Infinity;

        Enemy closestEnemy = null;

        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance > Data.AttackRadius)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;

                closestEnemy = enemy;
            }
        }

        _target = closestEnemy;
    }

    private void Attack()
    {
        if (_target == null)
            return;

        _attackTimer += Time.deltaTime;

        if (_attackTimer < Data.AttackSpeed)
            return;

        _attackTimer = 0f;

        float damage = Random.Range(
            Data.DamageRange.x,
            Data.DamageRange.y
        );

        Debug.DrawLine(
            transform.position,
            _target.transform.position,
            Color.yellow,
            0.1f
        );

        _target.TakeDamage(damage);
    }

    public bool CanUpgrade()
    {
        return Data.HasUpgrade;
    }

    public BuildsBase GetUpgradeData()
    {
        return Data.NextLevel;
    }

    public int GetSellPrice()
    {
        return Data.SalePrice;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Data == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            Data.AttackRadius
        );
    }
#endif
}