using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Coin>(out Coin coin))
        {
            int coinVal = coin.Collect();
            if (!IsServer) return;
            TotalCoins.Value += coinVal;
        }
    }

    public void SpendCoins(int coins)
    {
        TotalCoins.Value -= coins;
    }
}
