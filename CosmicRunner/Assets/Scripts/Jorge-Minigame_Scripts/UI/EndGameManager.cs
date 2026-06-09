using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace JorgeGame
{
    public class EndGameManager : MonoBehaviour
    {
        // Define victoria o derrota
        public bool isVictory;

        // UI final
        public GameObject coinsPanel;
        public TextMeshProUGUI coinsText;

        // Conexion API
        public JorgeCreditsApi jorgeCreditsApi;

        void Start()
        {
            SFXManager.instance.musicSource.Stop();

            if (isVictory)
            {
                SFXManager.instance.PlaySFX(SFXManager.instance.winSound, 0.2f);
                MinigameProgress.MarkBeaten(MinigameProgress.JorgeId);

                int coins = GameControl.Instance.coins;

                coinsText.text = "+" + coins;
                coinsPanel.SetActive(true);

                if (jorgeCreditsApi == null)
                {
                    jorgeCreditsApi = FindAnyObjectByType<JorgeCreditsApi>();
                }

                if (jorgeCreditsApi != null)
                {
                    jorgeCreditsApi.SendCredits(coins);
                }
            }
            else
            {
                SFXManager.instance.PlaySFX(SFXManager.instance.loseSound, 0.2f);

                coinsPanel.SetActive(false);
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene_Jorge");
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene_Jorge");
        }
    }
}