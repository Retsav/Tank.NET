using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    [SerializeField] private SpriteRenderer minimapPlayerIcon;
    
    [field:SerializeField] public Health Health { get; private set; }
    [field:SerializeField] public CoinWallet Wallet { get; private set; }
    [Header("Settings")] 
    [SerializeField] private int vCamOwnerPriority;
    [SerializeField] private Color playerColor; 

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerDespawned;
    public static event Action<TankPlayer> OnPlayerSpawned;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            PlayerName.Value = userData.userName;
            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            playerVCam.Priority = vCamOwnerPriority;
            minimapPlayerIcon.color = playerColor;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
