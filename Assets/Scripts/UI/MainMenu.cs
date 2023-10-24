using System;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text queueTimeText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text findMatchButtonText;


    private bool isMatchmaking;
    private bool isCancelling;


    private void Start()
    {
        if (ClientSingleton.Instance == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueStatusText.text = string.Empty;
        queueTimeText.text = string.Empty;
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
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            return;
        }
        ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        isMatchmaking = true;
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
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
