using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gio.Minigame
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Invasores del Espacio");
        }

        public void HelpButton()
        {
            Debug.Log("Falta agregar tutorial");
        }

        // Boton para salir del juego
        public void QuitButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameSelectScene");
        }
    }
}
