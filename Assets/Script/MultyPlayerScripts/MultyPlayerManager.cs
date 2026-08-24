using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    public Text textCountPlayers;
    public Text roomCode;
    public string adress;
    public InputField inputField;
    int playersCount = 0;
    public string serverName = "europe-central2";
    void Update()
    {
        PlayersCountCheck();
        UpdateUI();
    }
    void PlayersCountCheck()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            playersCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        }
        else if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            // Обычный клиент узнает количество игроков через список только из ID, который не равен null
            playersCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
        else
        {
            playersCount = 0;
        }
    }
    public void ExitServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
    void UpdateUI()
    {
        textCountPlayers.text = "Players count:" + playersCount.ToString();
    }
    //Кнопка для хоста
    public async void HostButton()
    {
        CreateRelay();
    }
    //Кнопка для клиента
    public void ClientButton()
    {
        //Debug.Log("ClientButton");
        JoinRelay(inputField.text);
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();//Добавляем игрока на сервера unity для дальнешей работы

        if (!AuthenticationService.Instance.IsSignedIn)//Проверяем то если игрок зашёл не анонимно
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();//Запускаем игрока анонимно
        }
    }
    async void CreateRelay()
    {

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2, serverName);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        roomCode.text = "Code: " + joinCode;

        // Исправлено для Unity 6
        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();
    }

    async void JoinRelay(string joinCode)
    {
        //Debug.Log("JoinRelay");
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode.ToUpper());

        // Исправлено для Unity 6
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        // Добавлено: старт клиента
        NetworkManager.Singleton.StartClient();
    }

    // 4. Кнопка загрузки сцены
    public void LoadScene(string sceneName)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
/*
 public class MultyPlayerManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text textCountPlayers;
    public Text roomCode;
    public string adress;
    public InputField inputField;
    int playersCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void SignAsHost()
    {   
        NetworkManager.Singleton.StartHost();
    }
    public void LoadScene(string sceneName)
    {
        if(playersCount == 2)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    public void ExitServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
    public void SignAsClient()
    {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.StartClient();
    }
    // Update is called once per frame
    void Update()
    {
        if(NetworkManager.Singleton.ConnectedClientsList != null)
        {
            playersCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        }
        UpdateUI();
    }
    void UpdateUI()
    {
        textCountPlayers.text = "Players count:" + playersCount.ToString();
        roomCode.text = "Code:".ToString();
    }
}
 */