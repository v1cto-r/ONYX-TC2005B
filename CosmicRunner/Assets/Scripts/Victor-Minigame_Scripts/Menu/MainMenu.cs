using UnityEngine;
using UnityEngine.SceneManagement;

namespace AB
{
    public class MainMenu : MonoBehaviour
    {
        // Boton para empezar o reanudar la partida
        public void PlayButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene_AB");
        }

        // Boton para abrir el tutorial o pantalla de ayuda
        public void HelpButton()
        {
            SceneManager.LoadScene("TutorialScene_AB");
        }

        // Boton para salir del juego
        public void QuitButton()
        {
            SceneManager.LoadScene("GameSelectScene");
        }
    }
}
