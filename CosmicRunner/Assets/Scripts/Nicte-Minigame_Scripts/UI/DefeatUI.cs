using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class DefeatUI : MonoBehaviour
{
    public TextMeshProUGUI finalDefeatScoreText;
    GeneralUI generalUI;

    void Start()
    {
        generalUI = FindObjectOfType<GeneralUI>();
        finalDefeatScoreText.text = GeneralUI.currentCredits.ToString();
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
