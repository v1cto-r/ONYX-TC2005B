using UnityEngine;
using TMPro;

namespace MECS
{
    // Controla la pantalla de fin de juego y el texto de resultado
    public class GameOverControl : MonoBehaviour
    {
        // Referencia al texto que muestra el mensaje de resultados
        [SerializeField] private TextMeshProUGUI resultText;

        // Texto que muestra el score final del jugador
        [SerializeField] private TextMeshProUGUI scoreText;

        // Referencia al SFXManager para reproducir los sonidos de resultado
        // private SFXManager sfxManager;

        // Decide el resultado final y actualiza los textos al cargar la escena
        void Start()
        {
            // Verifica si el jugador ha ganado o perdido y actualiza el texto en consecuencia

            // Si el jugador no perdio ni una sola vida
            if (GameControl.Instance.currentPrompts >= GameControl.Instance.promptsToWin)
            {
                // Mensaje de victoria cuando se completo el objetivo
                resultText.text = "VICTORIA";

                if (GameControl.Instance != null && GameControl.Instance.sfxManager != null)
                {
                    GameControl.Instance.sfxManager.PlayWinSound();
                }

            }

            // Si el jugador perdio todas sus vidas
            else
            {
                // Mensaje de derrota cuando no se llego a la meta
                resultText.text = "DERROTA";

                if (GameControl.Instance != null && GameControl.Instance.sfxManager != null)
                {
                    GameControl.Instance.sfxManager.PlayLoseSound();
                }
            }

            // Actualiza el texto del puntaje final
            scoreText.text = "+" + GameControl.Instance.currentScore.ToString();

        }
    }
}
