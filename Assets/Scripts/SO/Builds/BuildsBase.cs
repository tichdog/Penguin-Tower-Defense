using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "BuildsBase", menuName = "Scriptable Objects/BuildsBase")]
public class BuildsBase : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private LocalizedString _name;
    [SerializeField] private LocalizedString _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;

    [Header("Damage")]
    [SerializeField] private Vector2 _damageRange;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _attackRadius = 4f;

    [Header("Type")]
    [SerializeField] private BuildType _damageType;
    [SerializeField] private SpecialSkillType _specialSkill;

    [Header("Economy")]
    [SerializeField] private int _purchasePrice = 50;
    [SerializeField] private int _salePrice = 25;

    [Header("Progression")]
    [SerializeField] private float _constructionTime = 1f;
    [SerializeField] private int _level = 1;

    public LocalizedString Name => _name;
    public LocalizedString Description => _description;
    public Sprite Icon => _icon;
    public GameObject Prefab => _prefab;
    public Vector2 DamageRange => _damageRange;
    public float AttackSpeed => _attackSpeed;
    public float AttackRadius => _attackRadius;
    public BuildType DamageType => _damageType;
    public SpecialSkillType SpecialSkill => _specialSkill;
    public int PurchasePrice => _purchasePrice;
    public int SalePrice => _salePrice;
    public float ConstructionTime => _constructionTime;
    public int Level => _level;
}
