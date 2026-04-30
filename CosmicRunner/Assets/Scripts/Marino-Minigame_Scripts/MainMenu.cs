using UnityEngine;
using UnityEngine.SceneManagement;

namespace MECS
{
    public class MainMenu : MonoBehaviour
    {
        // Boton para empezar o reanudar la partida
        public void PlayButton()
        {
            // Dejamos el tiempo normal antes de cargar la escena de juego
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }

        // Boton para abrir el tutorial o pantalla de ayuda
        public void HelpButton()
        {
            SceneManager.LoadScene("TutorialScene");
        }

        // Boton para salir del juego
        public void QuitButton()
        {
            // Mensaje util para probar el boton en el editor
            Debug.Log("Game Quitted");

            // Aqui iria Application.Quit si quisieramos cerrar la app real
            // SceneManager.LoadScene("LevelSelectScene");
        }
    }
    }
