using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private NetworkObject playerPrefab;

    private ApplicationData appData;
    
    private const int DedicatedServerTargetFrameRate = 60;
    private const string GameSceneName = "Game";
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            Application.targetFrameRate = DedicatedServerTargetFrameRate;
            appData = new ApplicationData();
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);
            ClientSingleton clientSingleton =Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();
            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);
        Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}
