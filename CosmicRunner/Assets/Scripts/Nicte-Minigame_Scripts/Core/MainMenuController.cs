using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName="Game";
    public string helpSceneName="Help";
    public GameObject mainPanel;
    public GameObject helpPanel;

    void Start()
    {
        Time.timeScale=1f;
        mainPanel.SetActive(true);
        helpPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Help()
    {
        mainPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Salir del juego");
    }
}
