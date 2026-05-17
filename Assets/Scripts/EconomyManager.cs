using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [SerializeField] private int startCoins = 100;

    public int Coins { get; private set; }

    public event Action<int> OnCoinsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Coins = startCoins;
    }

    private void Start()
    {
        NotifyCoinsChanged();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;

        NotifyCoinsChanged();
    }

    public bool TrySpend(int amount)
    {
        if (Coins < amount)
            return false;

        Coins -= amount;

        NotifyCoinsChanged();

        return true;
    }

    private void NotifyCoinsChanged()
    {
        OnCoinsChanged?.Invoke(Coins);
    }
}