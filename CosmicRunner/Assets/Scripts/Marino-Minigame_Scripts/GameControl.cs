using UnityEngine;
using UnityEngine.SceneManagement;

namespace MECS
{
    // Control principal del juego y estado
    public class GameControl : MonoBehaviour
    {
        // Instancia global para consultar estado del juego desde otros sistemas
        public static GameControl Instance;

        [Header("Game Settings")]
        // Cantidad de prompts necesaria para ganar
        public int promptsToWin = 8;
        // Prompts completados durante la partida
        public int currentPrompts = 0;
        // Puntaje acumulado actual
        public int currentScore = 0;

        [Header("UI Control")]
        // Tiempo total de la partida en segundos
        public float totalGameTime = 120f;
        // Tiempo restante en la partida actual
        public float remainingTime;
        // Referencia a la UI principal para refrescar textos
        public UIControl uiControl;

        [Header("SFX Control")]
        // Controlador global de sonidos del juego
        public SFXManager sfxManager;

        // Marca cuando la partida ya termino
        private bool gameOver;
        // Referencia interna al controlador de prompts
        private PromptsControl promptsControl;

        // Prepara referencias y arranca la partida desde cero
        private void Awake()
        {
            // Evita tener mas de un controlador vivo al mismo tiempo
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Guardamos la instancia activa para otros scripts
            Instance = this;
            if (sfxManager == null)
            {
                sfxManager = FindAnyObjectByType<SFXManager>();
            }
            SetReferences();
            StartGame();
        }

        // Cuenta el tiempo y termina la partida al llegar a cero
        private void Update()
        {
            // Si el juego ya termino, no seguimos contando tiempo
            if (gameOver)
            {
                return;
            }

            // Restamos el delta de cada frame al temporizador
            remainingTime -= Time.deltaTime;

            // La UI siempre debe mostrar el tiempo actual
            if (uiControl != null)
            {
                uiControl.SetTimer(remainingTime);
            }

            // Si ya no queda tiempo, forzamos fin de partida
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                if (uiControl != null)
                {
                    uiControl.SetTimer(remainingTime);
                }
                EndGame();
            }
        }

        // Busca las referencias que usa el sistema de juego
        private void SetReferences()
        {
            // Tomamos el controlador de prompts si ya existe en escena
            if (promptsControl == null)
            {
                promptsControl = PromptsControl.Instance;
            }

            // Si la UI no esta ligada, la buscamos automaticamente
            if (uiControl == null)
            {
                uiControl = FindAnyObjectByType<UIControl>();
            }
        }

        // Reinicia valores y refresca la UI antes de jugar
        public void StartGame()
        {
            // Vaciamos score y progreso para empezar limpio
            currentScore = 0;
            currentPrompts = 0;
            remainingTime = totalGameTime;
            gameOver = false;

            // Mandamos el estado inicial a la interfaz
            if (uiControl != null)
            {
                uiControl.SetTimer(remainingTime);
                uiControl.SetScore(currentScore);
                uiControl.SetPrompts(currentPrompts, promptsToWin);
            }
        }

        // Suma puntos al marcador
        public void AddScore(int amount)
        {
            currentScore += amount;

            // Refrescamos la UI para mostrar el nuevo total
            if (uiControl != null)
            {
                uiControl.SetScore(currentScore);
            }
        }

        // Resta puntos sin permitir que el score baje de cero
        public void RemoveScore(int amount)
        {
            // Evitamos valores negativos en la interfaz y en la logica
            if (currentScore - amount < 0)
            {
                currentScore = 0;
            }
            else
            {
                currentScore -= amount;
            }

            // Actualizamos el texto de score despues de cambiarlo
            if (uiControl != null)
            {
                uiControl.SetScore(currentScore);
            }
        }

        // Incrementa el contador de prompts resueltos
        public void AddCompletedPrompt()
        {
            currentPrompts += 1;

            // La UI debe reflejar el progreso hacia la victoria
            if (uiControl != null)
            {
                uiControl.SetPrompts(currentPrompts, promptsToWin);
            }
        }

        // Marca el juego como terminado y carga la escena final
        private void EndGame()
        {
            gameOver = true;
            SceneManager.LoadScene("EndScene_MECS");
        }
    }
}