using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyData _data;
    private EnemyPath _path;

    [SerializeField] private int _currentPointIndex;
    [SerializeField] private float _currentHP;

    [Header("Health Bar")]
    [SerializeField] private float healthBarVerticalPadding = 0.16f;
    [SerializeField] private Vector2 healthBarSize = new Vector2(0.85f, 0.08f);
    [SerializeField] private Color healthBarBackColor = new Color(0.08f, 0.08f, 0.08f, 0.9f);
    [SerializeField] private Color healthBarFillColor = new Color(0.18f, 0.95f, 0.28f, 1f);
    [SerializeField] private Color healthBarLowColor = new Color(1f, 0.24f, 0.18f, 1f);

    private bool _isInitialized;
    private Transform healthBarRoot;
    private Transform healthBarFill;
    private SpriteRenderer spriteRenderer;
    private static Sprite healthBarSprite;

    public bool IgnoreDefenders => _data.IgnoreDefenders;

    public float CurrentHP => _currentHP;
    public float MaxHP => _data != null ? _data.MaxHP : 0f;

    public void Initialize(EnemyData data, EnemyPath path)
    {
        if (data == null || path == null || path.Points == null || path.Points.Length == 0)
        {
            Debug.LogWarning("[Enemy] Enemy data or path is missing.");
            Destroy(gameObject);
            return;
        }

        _data = data;

        _path = path;

        _currentHP = data.MaxHP;

        transform.position = _path.Points[0].position;

        _currentPointIndex = 1;

        _isInitialized = true;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CreateHealthBar();
        UpdateHealthBar();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.RegisterEnemy(this);
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        Move();
        UpdateHealthBarPosition();
    }

    private void Move()
    {
        if (_currentPointIndex >= _path.Points.Length)
        {
            ReachFinish();

            return;
        }

        Transform target = _path.Points[_currentPointIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            _data.Speed * Time.deltaTime
        );

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= 0.1f)
        {
            _currentPointIndex++;
        }
    }

    private void ReachFinish()
    {
        if (PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.TakeDamage(_data.Damage);

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.UnregisterEnemy(this);

        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        damage -= _data.Armor;

        if (damage < 1f)
            damage = 1f;

        _currentHP -= damage;
        _currentHP = Mathf.Max(0f, _currentHP);
        UpdateHealthBar();

        if (_currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.AddCoins(_data.GetReward());

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.UnregisterEnemy(this);

        Destroy(gameObject);
    }

    private void CreateHealthBar()
    {
        if (healthBarRoot != null)
            return;

        Sprite barSprite = GetHealthBarSprite();
        int sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 20 : 20;

        GameObject root = new GameObject("HealthBar");
        healthBarRoot = root.transform;
        UpdateHealthBarPosition();

        SpriteRenderer back = CreateHealthBarPart("Back", healthBarRoot, barSprite, healthBarBackColor, sortingOrder);
        back.transform.localScale = new Vector3(healthBarSize.x, healthBarSize.y, 1f);

        SpriteRenderer fill = CreateHealthBarPart("Fill", healthBarRoot, barSprite, healthBarFillColor, sortingOrder + 1);
        healthBarFill = fill.transform;
    }

    private SpriteRenderer CreateHealthBarPart(
        string objectName,
        Transform parent,
        Sprite sprite,
        Color color,
        int sortingOrder
    )
    {
        GameObject part = new GameObject(objectName);
        part.transform.SetParent(parent, false);

        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return renderer;
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null || _data == null || _data.MaxHP <= 0f)
            return;

        float healthPercent = Mathf.Clamp01(_currentHP / _data.MaxHP);
        healthBarFill.localScale = new Vector3(
            healthBarSize.x * healthPercent,
            healthBarSize.y,
            1f
        );

        healthBarFill.localPosition = new Vector3(
            -healthBarSize.x * (1f - healthPercent) * 0.5f,
            0f,
            -0.01f
        );

        SpriteRenderer fillRenderer = healthBarFill.GetComponent<SpriteRenderer>();
        if (fillRenderer != null)
            fillRenderer.color = healthPercent <= 0.3f ? healthBarLowColor : healthBarFillColor;
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBarRoot == null)
            return;

        Vector3 position = transform.position;

        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            position = new Vector3(
                bounds.center.x,
                bounds.max.y + healthBarVerticalPadding,
                transform.position.z
            );
        }
        else
        {
            position.y += healthBarVerticalPadding + 0.5f;
        }

        healthBarRoot.position = position;
        healthBarRoot.rotation = Quaternion.identity;
        healthBarRoot.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        if (healthBarRoot != null)
            Destroy(healthBarRoot.gameObject);
    }

    private static Sprite GetHealthBarSprite()
    {
        if (healthBarSprite != null)
            return healthBarSprite;

        Texture2D texture = Texture2D.whiteTexture;
        healthBarSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            texture.width
        );

        return healthBarSprite;
    }
}
