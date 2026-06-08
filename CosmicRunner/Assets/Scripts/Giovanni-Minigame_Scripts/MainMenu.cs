using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gio.Minigame
{
    public class MainMenu : MonoBehaviour
    {
        void Start()
        {
            if (SFXManager.Instance != null && SFXManager.Instance.musicaFondo1 != null) 
        {
            SFXManager.Instance.PlayBackgroundMusic(SFXManager.Instance.musicaFondo2); 
        } 
        }
        public void PlayButton()
        {
            Time.timeScale = 1f;
            if (SFXManager.Instance != null) SFXManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene("Invasores del Espacio");
        }

        public void HelpButton()
        {
            if (SFXManager.Instance != null) SFXManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene("IETutorial");
        }

        // Boton para salir del juego
        public void QuitButton()
        {
            Time.timeScale = 1f;
            if (SFXManager.Instance != null) SFXManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene("GameSelectScene");
        }
    }
}
