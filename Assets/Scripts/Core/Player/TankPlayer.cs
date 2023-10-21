using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    [Header("Settings")] 
    [SerializeField] private int vCamOwnerPriority;
    
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerVCam.Priority = vCamOwnerPriority;
        }
    }
}
