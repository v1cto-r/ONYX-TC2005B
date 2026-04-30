using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class PauseManager : MonoBehaviour
{
    // panel del menu de pausa
    public GameObject pauseMenu;

    private bool isPaused = false;

    public void TogglePause()
    {
        // cambia entre pausa y juego normal
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        // reanuda el juego
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        // asegura que el tiempo vuelva a la normalidad
        Time.timeScale = 1f;

        // oculta el menu de pausa
        pauseMenu.SetActive(false);

        // reactiva al jugador por seguridad
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.SetActive(true);

        // recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        // regresa al menu principal
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
}
}