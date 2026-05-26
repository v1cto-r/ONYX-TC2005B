using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace AB
{
    public class UIController : MonoBehaviour
    {
        public TMP_Text timeText;
        public TMP_Text creditsText;
        public GameObject[] bullets;
        public GameObject pauseMenu;

        public void ModifyCreditsText(int credits)
        {
            creditsText.text = credits.ToString();
        }

        public void UpdateTimeText(float elapsedTime, float gameDuration)
        {
            float remainingTime = Mathf.Max(0, gameDuration - elapsedTime);
            timeText.text = remainingTime.ToString("F1") + "s";
        }

        public void UpdateBullets(int bulletsCount)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].SetActive(i < bulletsCount);
            }
        }

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
