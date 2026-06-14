using System;
using System.Collections.Generic;
using UnityEngine;

public class DefenderUnit : MonoBehaviour
{
    private static readonly List<DefenderUnit> activeUnits = new();
    private static Sprite defaultSprite;

    [SerializeField] private float maxHealth = 12f;
    [SerializeField] private float movementSpeed = 2.2f;
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 0.45f;
    [SerializeField] private float aggroRange = 3.5f;
    [SerializeField] private float lifeTime;
    [SerializeField] private float returnDistance = 0.08f;

    private Enemy target;
    private Vector3 homePosition;
    private float currentHealth;
    private float attackTimer;
    private float lifeTimer;
    private bool isInitialized;

    public static IReadOnlyList<DefenderUnit> ActiveUnits => activeUnits;
    public event Action<DefenderUnit> Removed;
    public bool IsAlive => currentHealth > 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
        homePosition = transform.position;

        if (!activeUnits.Contains(this))
            activeUnits.Add(this);
    }

    public void Initialize(BarracksTowerData data, Vector3 home)
    {
        if (data != null)
        {
            maxHealth = data.UnitMaxHealth;
            movementSpeed = data.UnitMoveSpeed;
            damage = data.UnitDamage;
            attackCooldown = data.UnitAttackCooldown;
            attackRange = data.UnitAttackRange;
            aggroRange = data.UnitAggroRange;
            lifeTime = data.UnitLifeTime;
        }

        currentHealth = maxHealth;
        homePosition = home;
        lifeTimer = 0f;
        attackTimer = attackCooldown;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            Initialize(null, transform.position);

        if (lifeTime > 0f)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifeTime)
            {
                Die();
                return;
            }
        }

        if (target == null || target.CurrentHP <= 0f || target.IgnoreDefenders)
            target = FindClosestEnemy();

        if (target != null)
        {
            FightTarget();
            return;
        }

        ReturnHome();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || currentHealth <= 0f)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
            Die();
    }

    public static DefenderUnit FindClosest(Vector3 position, float radius)
    {
        DefenderUnit closest = null;
        float closestSqrDistance = radius * radius;

        for (int i = activeUnits.Count - 1; i >= 0; i--)
        {
            DefenderUnit unit = activeUnits[i];
            if (unit == null)
            {
                activeUnits.RemoveAt(i);
                continue;
            }

            if (!unit.IsAlive)
                continue;

            float sqrDistance = (unit.transform.position - position).sqrMagnitude;
            if (sqrDistance > closestSqrDistance)
                continue;

            closestSqrDistance = sqrDistance;
            closest = unit;
        }

        return closest;
    }

    public static DefenderUnit CreateDefault(Vector3 position)
    {
        GameObject unitObject = new GameObject("Default Defender Unit");
        unitObject.transform.position = position;
        unitObject.transform.localScale = Vector3.one * 0.45f;

        SpriteRenderer renderer = unitObject.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDefaultSprite();
        renderer.color = new Color(0.28f, 0.73f, 1f, 1f);
        renderer.sortingOrder = 3;

        CircleCollider2D collider = unitObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.45f;

        return unitObject.AddComponent<DefenderUnit>();
    }

    private Enemy FindClosestEnemy()
    {
        if (EnemyManager.Instance == null)
            return null;

        Enemy closest = null;
        float closestSqrDistance = aggroRange * aggroRange;

        foreach (Enemy enemy in EnemyManager.Instance.Enemies)
        {
            if (enemy == null || enemy.IgnoreDefenders)
                continue;

            float sqrDistance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance > closestSqrDistance)
                continue;

            closestSqrDistance = sqrDistance;
            closest = enemy;
        }

        return closest;
    }

    private void FightTarget()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                movementSpeed * Time.deltaTime
            );

            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer < attackCooldown)
            return;

        attackTimer = 0f;
        target.TakeDamage(damage);
    }

    private void ReturnHome()
    {
        float distance = Vector3.Distance(transform.position, homePosition);
        if (distance <= returnDistance)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            homePosition,
            movementSpeed * Time.deltaTime
        );
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        activeUnits.Remove(this);
        Removed?.Invoke(this);
        Removed = null;
    }

    private static Sprite GetDefaultSprite()
    {
        if (defaultSprite != null)
            return defaultSprite;

        Texture2D texture = Texture2D.whiteTexture;
        defaultSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            texture.width
        );

        return defaultSprite;
    }
}
