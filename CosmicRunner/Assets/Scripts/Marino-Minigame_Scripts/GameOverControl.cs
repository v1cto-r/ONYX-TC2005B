using UnityEngine;
using TMPro;

public class GameOverControl : MonoBehaviour
{
    // Referencia al texto que muestra el mensaje de resultados
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private TextMeshProUGUI scoreText;

    // Referencia al SFXManager para reproducir los sonidos de resultado
    // private SFXManager sfxManager;

    // Start es llamado antes del primer frame update
    void Start()
    {
        // Obtener la referencia al SFXManager para reproducir el sonido de resultado
        // sfxManager = FindAnyObjectByType<SFXManager>();

        // Verifica si el jugador ha ganado o perdido y actualiza el texto en consecuencia

        // Si el jugador no perdio ni una sola vida
        if (GameControl.Instance.currentPrompts >= GameControl.Instance.promptsToWin)
        {
            resultText.text = "VICTORIA";
            // sfxManager.PlayWinSound();

        }

        // Si el jugador perdio todas sus vidas
        else
        {
            resultText.text = "DERROTA";
            // sfxManager.PlayLoseSound();
        }

        // Actualiza el texto del puntaje final
        scoreText.text = "+" + GameControl.Instance.currentScore.ToString();

    }
}
