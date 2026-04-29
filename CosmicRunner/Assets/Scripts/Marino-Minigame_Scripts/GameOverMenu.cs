using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Metodo para el boton de reintentar en el menu de game over
    public void ReplayButton()
    {
        // Reinicia el tiempo de juego a 1 para asegurarse de que el juego no este pausado al reiniciar
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    // Metodo para el boton de menu principal en el menu de game over
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
