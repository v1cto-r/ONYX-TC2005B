using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class MainMenuManager : MonoBehaviour
{
    // carga la escena principal del minijuego
    public void StartGame()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté normal al iniciar el juego
        SceneManager.LoadScene("GameScene_Jorge");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté normal al salir del juego
        SceneManager.LoadScene("GameSelectScene");
    }
}
}