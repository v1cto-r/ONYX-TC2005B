using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Nicte.Minigame{
public class GeneralUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI creditsText;
    public GameObject pauseScreen;
     bool gamePaused = false;
    public static int currentCredits = 0;

    void Update()
    {
        if (GameController.instancia.currentState != "Playing")
        {
            return;
        }

        float timeRemaining = GameController.instancia.timeRemaining;
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateCredits(int credits)
    {
        SFXManager.Instance.CoinSound();
        currentCredits += credits;
        creditsText.text =currentCredits.ToString();
    }

    public void pause()
    {   
        if (GameController.instancia == null)
        {
            return;
        }

        if (GameController.instancia.currentState != "Playing")
        {
            return;
        }

        Time.timeScale = 0f;

        gamePaused = !gamePaused;

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(gamePaused);
        }
    }

    public void resumeGame()
    {
        if (GameController.instancia != null)
        {
            Time.timeScale = 1f;
        }

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
        gamePaused = false;
    }

    public void restartGame()
    {
        Time.timeScale = 1f;
        if (GameController.instancia != null)
        {
            GameController.instancia.currentState = "Playing";
            GameController.instancia.timeRemaining = GameController.instancia.time;
        }
        currentCredits = 0;
        pauseScreen.SetActive(false);
        SceneManager.LoadScene("AtaqueEstelarGame");
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("AtaqueEstelarGameStart");
    }
}
}
