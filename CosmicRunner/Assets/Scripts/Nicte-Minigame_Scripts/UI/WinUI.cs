using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class WinUI : MonoBehaviour
{
    public TextMeshProUGUI finalWinScoreText;
    void Start()
    {
        SFXManager.Instance.WinSound();
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
