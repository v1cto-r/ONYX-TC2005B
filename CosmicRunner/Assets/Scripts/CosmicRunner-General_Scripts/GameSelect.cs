using UnityEngine;

public class GameSelect : MonoBehaviour
{
    public void NicteMinigameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("AtaqueEstelarGameStart");
    }

    public void JorgeMinigameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_Jorge");
    }

    public void MarinoMinigameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_MECS");
    }

    public void VictorMinigameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene_AB");
    }

    public void GioMinigameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InvasoresMenu");
    }

    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}
