using System;
using UnityEngine;

public class PlayerCoinHandler : MonoBehaviour
{
    public int coin;
    public event Action<int> OnCoinChanged;

    public bool HasCoin(int amount) => coin >= amount;

    public void AddCoin(int amount)
    {
        coin += amount;
        OnCoinChanged?.Invoke(coin);
    }
    public bool TrySpend(int amount)
    {
        if (coin < amount) return false;
        coin -= amount;
        OnCoinChanged?.Invoke(coin);
        return true;
    }
    public void ResetCoin()
    {
        coin = 0;
        OnCoinChanged?.Invoke(coin);
    }
}
