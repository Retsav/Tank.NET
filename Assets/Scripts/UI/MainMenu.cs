using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text queueTimeText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private Toggle teamToggle;
    [SerializeField] private Toggle privateLobbyToggle;


    private bool isMatchmaking;
    private bool isCancelling;
    private bool isBusy;
    private float timeInQueue;


    private void Start()
    {
        if (ClientSingleton.Instance == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueStatusText.text = string.Empty;
        queueTimeText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(timeInQueue);
            queueTimeText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }

    public async void FindMatchPressed()
    {
        if (isCancelling) return;
        if (isMatchmaking)
        {
            isCancelling = true;
            queueStatusText.text = "Cancelling...";
            await ClientSingleton.Instance.GameManager.CancelMatchMaking();
            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueTimeText.text = string.Empty;
            return;
        }
        if (isBusy) return;
        ClientSingleton.Instance.GameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        timeInQueue = 0f;
        isMatchmaking = true;
        isBusy = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
        }
    }
    
    public async void StartHost()
    {
        if (isBusy) return;
        isBusy = true;
        await HostSingleton.Instance.GameManager.StartHostAsync(privateLobbyToggle.isOn);
        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy) return;
        isBusy = true;
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
    
    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy)
            return;
        isBusy = true;
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        isBusy = false;
    }
}
