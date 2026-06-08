using UnityEngine;
using UnityEngine.SceneManagement;

namespace MECS
{
    public class GameOverMenu : MonoBehaviour
    {
        // Boton para reiniciar la partida desde la pantalla final
        public void ReplayButton()
        {
            // Nos aseguramos de que el juego vuelva a velocidad normal antes de cargar la escena
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene_MECS");
        }

        // Boton para volver al menu principal
        public void MainMenuButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene_MECS");
        }
    }
}
