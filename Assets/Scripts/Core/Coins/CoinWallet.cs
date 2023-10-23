using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private BountyCoin coinPrefab;
    [Header("Settings")] 
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;
    
    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;
    
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;
        playerHealth.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        playerHealth.OnDie -= HandleDie;
    }

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

    private void HandleDie(Health playerHealth)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100));
        int bountyCoinValue = bountyValue / bountyCoinCount;
        if (bountyCoinValue < minBountyCoinValue) return;
        for (int i = 0; i < bountyCoinValue; i++)
        {
           BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
           coinInstance.SetValue(bountyCoinValue);
           coinInstance.NetworkObject.Spawn();
        }
    }
    
    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
