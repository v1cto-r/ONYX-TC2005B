using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace AB
{
    public class UIController : MonoBehaviour
    {
        public TMP_Text timeText;
        public TMP_Text shieldText;
        public GameObject shieldIcon;
        public TMP_Text creditsText;
        public GameObject[] bullets;
        public GameObject pauseMenu;

        // Modifica el texto de créditos
        public void ModifyCreditsText(int credits)
        {
            creditsText.text = credits.ToString();
        }

        // Modifica el texto del tiempo restante
        public void UpdateTimeText(float elapsedTime, float gameDuration)
        {
            float remainingTime = Mathf.Max(0, gameDuration - elapsedTime);
            timeText.text = remainingTime.ToString("F1") + "s";
        }

        // Modifica el texto del tiempo restante del shield
        public void UpdateShieldTime(float elapsedShieldTime, float totalShieldDuration)
        {
            float remainingShieldTime = Mathf.Max(0, totalShieldDuration - elapsedShieldTime);
            if (remainingShieldTime > 0)
            {
                shieldText.text = remainingShieldTime.ToString("F1");
                shieldIcon.gameObject.SetActive(true);
            }
            else
            {
                shieldIcon.gameObject.SetActive(false);
            }
        }

        // Modifica el número de balas mostradas
        public void UpdateBullets(int bulletsCount)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].SetActive(i < bulletsCount);
            }
        }

        // Funciones para los botones del menú de pausa
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene_AB");
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene_AB");
        }

        public void PauseGame() {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }

        public void ResumeGame() {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
}
