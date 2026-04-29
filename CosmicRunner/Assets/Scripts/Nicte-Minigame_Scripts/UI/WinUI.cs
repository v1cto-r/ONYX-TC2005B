using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class WinUI : MonoBehaviour
{
    public TextMeshProUGUI finalWinScoreText;
    GeneralUI generalUI;

    void Start()
    {
        generalUI = FindObjectOfType<GeneralUI>();
        finalWinScoreText.text = GeneralUI.currentCredits.ToString();
    }
    public void playGame()
    {
            SceneManager.LoadScene("AtaqueEstelarGame");

    }
    
    public void backToMenu()
    {
            SceneManager.LoadScene("AtaqueEstelarGameStart");
    }
}
