using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text creditsText;

    void Start()
    {
        if (PlayerPrefs.GetInt("result") == 1)
        {
            resultText.text = "VICTORIA";
        }
        else
        {
            resultText.text = "DERROTA";
        }
        creditsText.text = "+" + PlayerPrefs.GetInt("collected_credits");
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteKey("collected_credits");
        PlayerPrefs.DeleteKey("result");
        SceneManager.LoadScene("GameScene_AB");
    }

    public void ExitToMenu()
    {
        PlayerPrefs.DeleteKey("collected_credits");
        PlayerPrefs.DeleteKey("result");
        SceneManager.LoadScene("MainMenuScene_AB");
    }
}
