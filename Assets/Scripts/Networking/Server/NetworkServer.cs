using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    public Action<string> OnClientLeft;
    
    
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, GameData> authIdToUserData = new Dictionary<string, GameData>();
    
    
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }



    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        GameData gameData = JsonUtility.FromJson<GameData>(payload);
        
        clientIdToAuth[request.ClientNetworkId] = gameData.userAuthId;
        authIdToUserData[gameData.userAuthId] = gameData;
        
        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }
    
    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId) {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
            OnClientLeft?.Invoke(authId);
        }
    }

    public GameData GetUserDataByClientID(ulong clientId)
    {
        if (!clientIdToAuth.TryGetValue(clientId, out string authId))
            return null;
        if (!authIdToUserData.TryGetValue(authId, out GameData userData))
            return null;
        return userData;
    }
    public void Dispose()
    {
        if (networkManager == null)
            return;
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted-= OnNetworkReady;
        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
