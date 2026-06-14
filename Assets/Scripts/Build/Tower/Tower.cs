using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public BuildsBase Data { get; private set; }

    public BuildNode Node { get; private set; }

    private Enemy _target;

    private float _attackTimer;
    private static Material attackLineMaterial;

    [Header("Attack Visual")]
    [SerializeField] private Color attackLineColor = new Color(1f, 0.87f, 0.18f, 1f);
    [SerializeField] private float attackLineWidth = 0.08f;
    [SerializeField] private float attackLineDuration = 0.12f;
    [SerializeField] private Vector3 attackLineOffset = new Vector3(0f, 0.2f, 0f);

    public virtual void Initialize(
        BuildsBase data,
        BuildNode node)
    {
        Data = data;
        Node = node;
    }

    private void Update()
    {
        if (Data == null)
            return;

        Tick();
    }

    protected virtual void Tick()
    {
        if (EnemyManager.Instance == null)
            return;

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

        ShowAttackLine(_target);

        _target.TakeDamage(damage);
    }

    public bool CanUpgrade()
    {
        return Data != null && Data.HasUpgrade;
    }

    public BuildsBase GetUpgradeData()
    {
        return Data != null ? Data.NextLevel : null;
    }

    public int GetSellPrice()
    {
        return Data != null ? Data.SalePrice : 0;
    }

    private void ShowAttackLine(Enemy target)
    {
        if (target == null)
            return;

        GameObject lineObject = new GameObject($"{name}_AttackLine");
        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = attackLineWidth;
        line.endWidth = attackLineWidth * 0.45f;
        line.startColor = attackLineColor;
        line.endColor = new Color(
            attackLineColor.r,
            attackLineColor.g,
            attackLineColor.b,
            0f
        );
        line.material = GetAttackLineMaterial();
        line.sortingOrder = 40;
        line.numCapVertices = 4;

        Vector3 start = transform.position + attackLineOffset;
        Vector3 end = target.transform.position;
        start.z = 0f;
        end.z = 0f;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        Destroy(lineObject, attackLineDuration);
    }

    private static Material GetAttackLineMaterial()
    {
        if (attackLineMaterial != null)
            return attackLineMaterial;

        Shader shader = Shader.Find("Sprites/Default");
        attackLineMaterial = new Material(shader);
        attackLineMaterial.name = "Runtime Attack Line Material";

        return attackLineMaterial;
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
