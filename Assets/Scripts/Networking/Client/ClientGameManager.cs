using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClientGameManager : IDisposable
{
    private const string MenuSceneName = "Menu";
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        networkClient = new NetworkClient(NetworkManager.Singleton);
        AuthState authState = await AuthenticationWrapper.DoAuth();
        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
        GameData gameData = new GameData
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(gameData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
        NetworkManager.Singleton.StartClient();
    }

    
    public void Disconnect()
    {
        networkClient.Disconnect();
    }
    public void Dispose()
    {
        networkClient?.Dispose();
    }

}
