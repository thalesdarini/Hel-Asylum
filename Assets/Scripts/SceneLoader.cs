using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas = null;
    [SerializeField] GameObject creditsCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Loader")
        {
            LoadNextScene();
        }
        if (SceneManager.GetActiveScene().name == "Outro")
        {
            SoundManager.ChangeMusic("song_final");
        }
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("Tutorial");
        SoundManager.ChangeMusic("song_game");
    }

    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
        SoundManager.ChangeMusic("song_theme");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowCreditsCanvas()
    {
        mainCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }
    public void ShowMenuCanvas()
    {
        mainCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
