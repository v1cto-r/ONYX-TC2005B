using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace JorgeGame
{
public class EndGameManager : MonoBehaviour
{
    // define si es victoria o derrota
    public bool isVictory;

    // referencias al panel y texto de monedas
    public GameObject coinsPanel;
    public TextMeshProUGUI coinsText;

    void Start()
    {
        if (isVictory)
        {
            // obtiene las monedas guardadas y las muestra
            int coins = GameControl.Instance.coins;
            coinsText.text = "+" + coins;
            coinsPanel.SetActive(true);
        }
        else
        {
            // en derrota no se muestran monedas
            coinsPanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        // reinicia el nivel actual
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void GoToMenu()
    {
        // regresa al menu principal
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
}
}