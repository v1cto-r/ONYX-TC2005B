using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
namespace Gio.Minigame
{
public class End : MonoBehaviour
{
    public TMP_Text resultado;
    public TMP_Text creditsText;

    void Start()
    {
        if (PlayerPrefs.GetInt("resultado") == 1)
        {
            resultado.text = "VICTORIA";
        }
        else
        {
            resultado.text = "DERROTA";
        }
        creditsText.text = "+" + PlayerPrefs.GetInt("credits");
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteKey("credits");
        PlayerPrefs.DeleteKey("resultado");
        SceneManager.LoadScene("Invasores del Espacio");
    }

    public void ExitToMenu()
    {
        PlayerPrefs.DeleteKey("credits");
        PlayerPrefs.DeleteKey("resultado");
        SceneManager.LoadScene("IEGameStart");
    }
}
}
