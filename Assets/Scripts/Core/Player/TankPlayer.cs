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
    [SerializeField] private Texture2D crosshair;
    
    [field:SerializeField] public Health Health { get; private set; }
    [field:SerializeField] public CoinWallet Wallet { get; private set; }
    [Header("Settings")] 
    [SerializeField] private int vCamOwnerPriority;
    [SerializeField] private Color playerColor; 

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

    public static event Action<TankPlayer> OnPlayerDespawned;
    public static event Action<TankPlayer> OnPlayerSpawned;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;
            if (IsHost)
            { 
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            }

            PlayerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;
            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            playerVCam.Priority = vCamOwnerPriority;
            minimapPlayerIcon.color = playerColor;
            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
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
