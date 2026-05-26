using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MECS
{
	// Control general de UI y paneles
	public class UIControl : MonoBehaviour
	{
		// Texto que muestra progreso de prompts completados
		[Header("UI Labels")]
		[SerializeField] private TMP_Text promptsProgressText;
		// Texto que muestra cuantas palabras guarda el jugador
		[SerializeField] private TMP_Text wordsStorageText;
		// Texto del score actual
		[SerializeField] private TMP_Text scoreText;
		// Texto del tiempo restante
		[SerializeField] private TextMeshProUGUI timerText;

		[Header("Prompt Panel")]
		// Panel que muestra la lista de prompts
		[SerializeField] private GameObject promptsPanel;
		// Boton que abre o cierra ese panel
		[SerializeField] private Button promptsPanelButton;
		// Icono del boton para alternar estado visual
		[SerializeField] private Image promptsPanelIconImage;
		// Sprite cuando el panel esta visible
		[SerializeField] private Sprite showPromptsSprite;
		// Sprite cuando el panel esta oculto
		[SerializeField] private Sprite hidePromptsSprite;

		[Header("Pause Panel")]
		// Panel de pausa
		[SerializeField] private GameObject pausePanel;

		[Header("Settings")]
		// Referencia al control del jugador para pausarlo o reactivarlo
		[SerializeField] private PlayerControl playerControl;

		[Header("Text Formats")]
		// Formato para el contador de palabras
		[SerializeField] private string wordsStorageFormat = "Palabras: {0}/{1}";
		// Formato para el score
		[SerializeField] private string scoreFormat = "Score: {0}";
		// Formato para el progreso de prompts
		[SerializeField] private string promptsProgressFormat = "#Prompts: {0}/{1}";
		// Estado de pausa del jugador guardado antes de pausar
		private bool playerControlWasEnabledBeforePause;
		// Indica si ya guardamos el estado del jugador
		private bool hasStoredPlayerControlState;
		// Marca si el juego esta pausado
		private bool isGamePaused;

		// Prepara enlaces basicos de UI y botones
		private void Awake()
		{
			// Si no asignamos el jugador, lo buscamos por nombre
			if (playerControl == null)
			{
				playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
			}

			// El boton del panel de prompts llama al toggle principal
			if (promptsPanelButton != null)
			{
				promptsPanelButton.onClick.AddListener(TogglePromptsPanel);

				// Si no hay icono asignado, intentamos encontrarlo dentro del boton
				if (promptsPanelIconImage == null)
				{
					Transform imageChild = promptsPanelButton.transform.Find("Image");
					if (imageChild != null)
					{
						promptsPanelIconImage = imageChild.GetComponent<Image>();
					}
				}
			}
		}

		// Inicializa paneles y pinta valores actuales
		private void Start()
		{
			// Por defecto, el panel de prompts arranca cerrado
			if (promptsPanel != null)
			{
				// Al ocultarlo, tambien limpiamos estados pendientes
				SetPromptsPanelVisible(false);
			}

			// Si ya existe el control principal, copiamos sus valores iniciales
			if (GameControl.Instance != null)
			{
				SetTimer(GameControl.Instance.remainingTime);
				SetScore(GameControl.Instance.currentScore);
				SetPrompts(GameControl.Instance.currentPrompts, GameControl.Instance.promptsToWin);
			}
		}

		// Asegura que la escena no quede pausada al activar la UI
		private void OnEnable()
		{
			SetPauseState(false);
		}

		// Limpia listeners y restaura el tiempo al salir de la escena
		private void OnDestroy()
		{
			// Quitamos el listener para evitar llamadas duplicadas
			if (promptsPanelButton != null)
			{
				promptsPanelButton.onClick.RemoveListener(TogglePromptsPanel);
			}

			// Por seguridad, devolvemos el tiempo a normal
			Time.timeScale = 1f;
		}

		// Muestra el tiempo restante con formato mm:ss
		public void SetTimer(float remainingTime)
		{
			// Sin label no hay nada que actualizar
			if (timerText == null)
			{
				return;
			}

			// Separamos minutos y segundos para que sea facil de leer
			int minutes = Mathf.FloorToInt(remainingTime / 60f);
			int seconds = Mathf.FloorToInt(remainingTime % 60f);
			timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
		}

		// Pinta el score actual en la UI
		public void SetScore(int score)
		{
			if (scoreText == null)
			{
				return;
			}

			scoreText.text = string.Format(scoreFormat, score);
		}

		// Pinta el progreso de prompts completados
		public void SetPrompts(int currentPrompts, int promptsToWin)
		{
			if (promptsProgressText == null)
			{
				return;
			}

			promptsProgressText.text = string.Format(promptsProgressFormat, currentPrompts, promptsToWin);
		}

		// Muestra cuantas palabras hay almacenadas
		public void SetWordsStorage(int currentWords, int maxWords)
		{
			if (wordsStorageText == null)
			{
				return;
			}

			wordsStorageText.text = string.Format(wordsStorageFormat, currentWords, maxWords);
		}

		// Indica si el panel de prompts esta abierto
		public bool IsPromptsPanelOpen()
		{
			return promptsPanel != null && promptsPanel.activeSelf;
		}

		// Alterna la visibilidad del panel de prompts
		public void TogglePromptsPanel()
		{
			// No permitimos cambios mientras el juego esta pausado
			if (isGamePaused)
			{
				return;
			}

			// Sin panel no hay nada que alternar
			if (promptsPanel == null)
			{
				return;
			}

			// Invertimos el estado actual
			SetPromptsPanelVisible(!promptsPanel.activeSelf);
		}

		// Fuerza el estado visible u oculto del panel de prompts
		public void SetPromptsPanelVisible(bool isVisible)
		{
			// Sin panel no podemos hacer nada
			if (promptsPanel == null)
			{
				return;
			}

			// Al cerrar el panel limpiamos todo el trabajo incompleto y sus mensajes
			if (!isVisible && PromptsControl.Instance != null)
			{
				PromptsControl.Instance.CancelAllUnsubmittedPromptFills();
				PromptsControl.Instance.HideAllQualityTexts();
			}

			promptsPanel.SetActive(isVisible);
			UpdatePromptsButtonIcon(isVisible);
		}

		// Cambia el icono del boton segun el estado del panel
		private void UpdatePromptsButtonIcon(bool isPanelVisible)
		{
			// Si no hay imagen, no hay nada que actualizar
			if (promptsPanelIconImage == null)
			{
				return;
			}

			// Elegimos el sprite correcto segun el estado
			if (isPanelVisible)
			{
				if (hidePromptsSprite != null)
				{
					promptsPanelIconImage.sprite = hidePromptsSprite;
				}
			}
			else
			{
				if (showPromptsSprite != null)
				{
					promptsPanelIconImage.sprite = showPromptsSprite;
				}
			}
		}

		// Pausa la partida y muestra el panel correspondiente
		public void PauseGame()
		{
			SetPauseState(true);
		}

		// Reanuda la partida desde pausa
		public void ResumeGame()
		{
			SetPauseState(false);
		}

		// Reinicia la escena del juego desde cero
		public void RestartGame()
		{
			SetPauseState(false);
			SceneManager.LoadScene("GameScene_MECS");
		}

		// Vuelve al menu principal
		public void QuitGame()
		{
			SetPauseState(false);
			SceneManager.LoadScene("MainMenuScene_MECS");
		}

		// Guarda y aplica el estado de pausa en un solo punto
		private void SetPauseState(bool isPaused)
		{
			// Guardamos la nueva marca de pausa y ajustamos el tiempo global
			isGamePaused = isPaused;
			Time.timeScale = isPaused ? 0f : 1f;

			// Si pausamos, cerramos tambien el panel de prompts para no dejar UI abierta
			if (isPaused)
			{
				SetPromptsPanelVisible(false);
			}

			// Durante la pausa deshabilitamos al jugador, y al salir restauramos su estado anterior
			if (playerControl != null)
			{
				if (isPaused)
				{
					playerControlWasEnabledBeforePause = playerControl.enabled;
					hasStoredPlayerControlState = true;
					playerControl.enabled = false;
				}
				else if (hasStoredPlayerControlState)
				{
					playerControl.enabled = playerControlWasEnabledBeforePause;
					if (playerControl.enabled)
					{
						playerControl.RefreshInputAfterPause();
					}
					hasStoredPlayerControlState = false;
				}
			}

			// Si existe panel de pausa, lo encendemos o apagamos segun corresponda
			if (pausePanel != null)
			{
				pausePanel.SetActive(isPaused);
			}
		}

	}
}
