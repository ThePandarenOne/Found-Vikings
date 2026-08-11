using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public AudioSource music;
    public GameObject pauseMenu;
    public Slider musicSlider;
    public GameObject winMenu;
    public Text winText;
    public void Pause()
    {
        if(pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    public void ChangeVolume(byte volume)
    {
        musicSlider.value = volume;
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void VictoryMenu(string side)
    {
        winMenu.SetActive(true);
        winText.text = "Victory by the " + side;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSlider.value = musicSlider.value/4;
        music.volume = music.volume/4;
    }

    // Update is called once per frame
    void Update()
    {
        if(music != null)
        {
            music.volume = musicSlider.value;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
}
