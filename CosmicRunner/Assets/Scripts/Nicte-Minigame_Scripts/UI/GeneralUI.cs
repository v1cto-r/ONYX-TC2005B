using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GeneralUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI creditsText;
    public GameObject pauseScreen;
    public GameObject startScreen;
    public GameObject menuScreen;
     bool gamePaused = false;
     public static int finalScore; 
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
        currentCredits += credits;
        creditsText.text =currentCredits.ToString();
        Debug.Log("Créditos actualizados: " + currentCredits);
    }

    public void pause()
    {
        Debug.Log("Pausa activada");
        
        if (GameController.instancia == null)
            return;

        if (GameController.instancia.currentState != "Playing")
            return;

        GameController.instancia.TogglePause();

        gamePaused = !gamePaused;

        if (pauseScreen != null)
            pauseScreen.SetActive(gamePaused);
    }

    public void resumeGame()
    {
        if (GameController.instancia != null)
        {
            GameController.instancia.TogglePause();
        }

        if (pauseScreen != null)
            pauseScreen.SetActive(false);

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
