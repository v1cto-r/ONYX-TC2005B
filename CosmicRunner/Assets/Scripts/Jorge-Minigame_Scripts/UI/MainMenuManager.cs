using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Musica del menu")]
        [SerializeField] private AudioSource menuMusic;

        // carga la escena principal del minijuego
        public void StartGame()
        {
            Time.timeScale = 1f;

            StopMenuMusic();

            SceneManager.LoadScene("GameScene_Jorge", LoadSceneMode.Single);
        }

        // regresa al selector de juegos
        public void ExitGame()
        {
            Time.timeScale = 1f;

            StopMenuMusic();

            SceneManager.LoadScene("GameSelectScene", LoadSceneMode.Single);
        }

        private void StopMenuMusic()
        {
            if (menuMusic != null)
            {
                menuMusic.Stop();
            }
        }
    }
}