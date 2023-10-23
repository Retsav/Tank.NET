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
    private MatchplayMatchmaker matchmaker;
    private UserData userData;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();
        AuthState authState = await AuthenticationWrapper.DoAuth();
        if (authState == AuthState.Authenticated)
        {
            userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
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
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
        NetworkManager.Singleton.StartClient();
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);
        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            //Connect to server
        }
        return matchmakingResult.result;
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
