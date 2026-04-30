using UnityEngine;

namespace JorgeGame
{
public class InstructionsManager : MonoBehaviour
{
    // panel de instrucciones
    public GameObject instructionsPanel;

    // referencia al menu de pausa
    public GameObject pauseMenu;

    void Start()
    {
        // mostrar instrucciones al iniciar
        instructionsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // cierra el panel de instrucciones y reanuda el juego
    public void CloseInstructions()
    {
        // oculta instrucciones
        instructionsPanel.SetActive(false);

        // reanuda el juego
        Time.timeScale = 1f;
    }

    // abre el panel de instrucciones y pausa la partida
    public void OpenInstructions()
    {
        // muestra instrucciones
        instructionsPanel.SetActive(true);

        // si viene desde pausa, oculta el menu de pausa
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Time.timeScale = 0f;
    }
}
}