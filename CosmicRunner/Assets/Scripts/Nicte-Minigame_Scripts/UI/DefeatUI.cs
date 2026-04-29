using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class DefeatUI : MonoBehaviour
{
    public TextMeshProUGUI finalDefeatScoreText;

    void Start()
    {
        SFXManager.Instance.DefeatSound();
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
