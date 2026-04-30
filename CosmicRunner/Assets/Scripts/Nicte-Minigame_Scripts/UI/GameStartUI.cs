using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nicte.Minigame{
public class GameStartUI : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject controlsScreen1;
    public GameObject controlsScreen2;

    void Start()
    {
        // Mostrar la pantalla de inicio y ocultar la UI general al iniciar
        if (startScreen != null)
            startScreen.SetActive(true);
        if (controlsScreen1 != null)
            controlsScreen1.SetActive(false);
    }

    public void playGame()
    {
        // Iniciar el juego y ocultar la pantalla de inicio
        if (startScreen != null)
            SceneManager.LoadScene("AtaqueEstelarGame");
    }
    public void controlScreen1()
    {
        if (startScreen != null)
            startScreen.SetActive(false);
        if (controlsScreen1 != null)
            controlsScreen1.SetActive(true);
        if (controlsScreen2 != null)
            controlsScreen2.SetActive(false);
    }

    public void controlScreen2()
    {
        if (controlsScreen1 != null)
            controlsScreen1.SetActive(false);
        if (controlsScreen2 != null)
            controlsScreen2.SetActive(true);
        if (startScreen != null)
            startScreen.SetActive(false);
    }

    public void closeControls()
    {
        if (controlsScreen2 != null)
            controlsScreen2.SetActive(false);
        if (startScreen != null)
            startScreen.SetActive(true);
    }

    public void exitGame()
    {
        //Para regresar al menu con los demas minijuegos
    }
}
}