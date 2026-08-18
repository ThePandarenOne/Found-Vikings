using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
public class MultyPlayerManager : MonoBehaviour
{
    public Text textCountPlayers;
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
        textCountPlayers.text = "Players count:" + playersCount.ToString();
    }
}
