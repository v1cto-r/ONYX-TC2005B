using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameSelectScene");
    }

    public void OpenCustomize()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CustomizeScene");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("LogInScene");
    }
}