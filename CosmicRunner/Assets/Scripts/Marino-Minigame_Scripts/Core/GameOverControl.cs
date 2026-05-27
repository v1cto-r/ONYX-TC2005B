using UnityEngine;
using TMPro;
using System.Collections;

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
        private SFXManager sfxManager;
        // Servicio simple del API del minijuego
        private MinigameApiService apiService;

        [Header("API Settings")]
        // Base del API del minijuego
        [SerializeField] private string apiBaseUrl = "https://localhost:12003/minigame";
        // Id del usuario a reportar al API
        [SerializeField] private int userId = 1;

        // Decide el resultado final y actualiza los textos al cargar la escena
        void Start()
        {
            // Buscamos el SFXManager en la escena para reproducir los sonidos de resultado
            sfxManager = FindAnyObjectByType<SFXManager>();
            apiService = new MinigameApiService(apiBaseUrl);

            // Verifica si el jugador ha ganado o perdido y actualiza el texto en consecuencia

            // Si el jugador gano
            if (GameControl.LastRunWasWin)
            {
                // Mensaje de victoria cuando se completo el objetivo
                resultText.text = "VICTORIA";
                sfxManager.PlayWinSound();
                
            }

            // Si el jugador perdio
            else
            {
                // Mensaje de derrota cuando no se llego a la meta
                resultText.text = "DERROTA";
                resultText.color = Color.lightGray; // Cambia el color del texto a gris claro para indicar derrota
                sfxManager.PlayLoseSound();
            }

            // Actualiza el texto del puntaje final
            scoreText.text = "+" + GameControl.LastRunScore.ToString();

            // Envia el puntaje al API del minijuego para que lo registre en el backend
            StartCoroutine(SendCredits());

        }

        private IEnumerator SendCredits()
        {
            if (apiService == null)
            {
                yield break;
            }

            yield return apiService.PostCredits(userId, GameControl.LastRunScore);
        }
    }
}
