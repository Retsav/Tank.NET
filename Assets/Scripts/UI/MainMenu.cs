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


    private void Start()
    {
        if (ClientSingleton.Instance == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        queueStatusText.text = string.Empty;
        queueTimeText.text = string.Empty;
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
