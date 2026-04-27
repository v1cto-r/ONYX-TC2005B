using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControl : MonoBehaviour
{
	[Header("UI Labels")]
	[SerializeField] private TMP_Text wordsStorageText;
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TextMeshProUGUI timerText;

	[Header("Prompt Panel")]
	[SerializeField] private GameObject promptsPanel;
	[SerializeField] private Button promptsPanelButton;
	[SerializeField] private Image promptsPanelIconImage;
	[SerializeField] private Sprite showPromptsSprite;
	[SerializeField] private Sprite hidePromptsSprite;

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pausePanelButton;

    [Header("Settings")]
    [SerializeField] private int wordsStorageCapacity = 4;
    [SerializeField] private float remainingTime = 120f;
	[SerializeField] private PlayerControl playerControl;

	[Header("Text Formats")]
	[SerializeField] private string wordsStorageFormat = "Palabras: {0}/{1}";
	[SerializeField] private string scoreFormat = "Score: {0}";

	private PromptsControl promptsControl;
	private bool playerControlWasEnabledBeforePause;
	private bool hasStoredPlayerControlState;
	private bool isGamePaused;

	private void Awake()
	{
		promptsControl = PromptsControl.Instance;

		if (playerControl == null)
		{
			playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
		}

		if (promptsPanelButton != null)
		{
			promptsPanelButton.onClick.AddListener(TogglePromptsPanel);

			if (promptsPanelIconImage == null)
			{
				Transform imageChild = promptsPanelButton.transform.Find("Image");
				if (imageChild != null)
				{
					promptsPanelIconImage = imageChild.GetComponent<Image>();
				}
			}
		}

		if (pausePanelButton != null)
		{
			pausePanelButton.onClick.AddListener(PauseGame);
		}
	}

    private void Start()
    {
        RefreshUI();

		if (promptsPanel != null)
		{
			SetPromptsPanelVisible(false);
		}
    }

	private void OnEnable()
	{
		if (promptsControl == null)
		{
			promptsControl = PromptsControl.Instance;
		}

		SetPauseState(false);

		RefreshUI();
	}

	private void OnDestroy()
	{
		if (promptsPanelButton != null)
		{
			promptsPanelButton.onClick.RemoveListener(TogglePromptsPanel);
		}

		if (pausePanelButton != null)
		{
			pausePanelButton.onClick.RemoveListener(PauseGame);
		}

		Time.timeScale = 1f;
	}

	private void Update()
	{
		if (promptsControl == null && PromptsControl.Instance != null)
		{
			promptsControl = PromptsControl.Instance;
		}

		// Si el tiempo restante es mayor a 0, decrementarlo con el tiempo entre frames
		if (remainingTime > 0)
		{
			remainingTime -= Time.deltaTime;
		}

		// Si el tiempo restante es menor a 0, establecerlo en 0 para evitar valores negativos
		else if (remainingTime < 0)
		{
			remainingTime = 0;
		}

		// Incrementa el tiempo transcurrido con el tiempo entre frames
		int minutes = Mathf.FloorToInt(remainingTime / 60);
		int seconds = Mathf.FloorToInt(remainingTime % 60);

		// Actualiza el texto del temporizador con el formato MM:SS:MS
		if (timerText != null)
		{
			timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
		}

		RefreshUI();
	}

	public void TogglePromptsPanel()
	{
		if (isGamePaused)
		{
			return;
		}

		if (promptsPanel == null)
		{
			return;
		}

		SetPromptsPanelVisible(!promptsPanel.activeSelf);
	}

	public void SetPromptsPanelVisible(bool isVisible)
	{
		if (promptsPanel == null)
		{
			return;
		}

		promptsPanel.SetActive(isVisible);
		UpdatePromptsButtonIcon(isVisible);
	}

	private void UpdatePromptsButtonIcon(bool isPanelVisible)
	{
		if (promptsPanelIconImage == null)
		{
			return;
		}

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

	public void PauseGame()
	{
		SetPauseState(true);
	}

	public void ResumeGame()
	{
		SetPauseState(false);
	}

	private void SetPauseState(bool isPaused)
	{
		isGamePaused = isPaused;
		Time.timeScale = isPaused ? 0f : 1f;

		if (isPaused)
		{
			SetPromptsPanelVisible(false);
		}

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
				hasStoredPlayerControlState = false;
			}
		}

		if (pausePanel != null)
		{
			pausePanel.SetActive(isPaused);
		}
	}

	private void RefreshUI()
	{
		if (promptsControl == null)
		{
			return;
		}

		if (wordsStorageText != null)
		{
			wordsStorageText.text = string.Format(
				wordsStorageFormat,
				promptsControl.CurrentWordCount,
				wordsStorageCapacity
			);
		}

		if (scoreText != null)
		{
			scoreText.text = string.Format(scoreFormat, promptsControl.TotalScore);
		}
	}
}
