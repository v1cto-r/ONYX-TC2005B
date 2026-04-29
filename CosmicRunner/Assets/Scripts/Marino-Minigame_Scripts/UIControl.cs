using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControl : MonoBehaviour
{
	[Header("UI Labels")]
	[SerializeField] private TMP_Text promptsProgressText;
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
	[SerializeField] private PlayerControl playerControl;

	[Header("Text Formats")]
	[SerializeField] private string wordsStorageFormat = "Palabras: {0}/{1}";
	[SerializeField] private string scoreFormat = "Score: {0}";
	[SerializeField] private string promptsProgressFormat = "#Prompts: {0}/{1}";
	private bool playerControlWasEnabledBeforePause;
	private bool hasStoredPlayerControlState;
	private bool isGamePaused;

	private void Awake()
	{
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
		if (promptsPanel != null)
		{
			SetPromptsPanelVisible(false);
		}

		if (GameControl.Instance != null)
		{
			SetTimer(GameControl.Instance.remainingTime);
			SetScore(GameControl.Instance.currentScore);
			SetPrompts(GameControl.Instance.currentPrompts, GameControl.Instance.promptsToWin);
		}
    }

	private void OnEnable()
	{
		SetPauseState(false);
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

	public void SetTimer(float remainingTime)
	{
		if (timerText == null)
		{
			return;
		}

		int minutes = Mathf.FloorToInt(remainingTime / 60f);
		int seconds = Mathf.FloorToInt(remainingTime % 60f);
		timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
	}

	public void SetScore(int score)
	{
		if (scoreText == null)
		{
			return;
		}

		scoreText.text = string.Format(scoreFormat, score);
	}

	public void SetPrompts(int currentPrompts, int promptsToWin)
	{
		if (promptsProgressText == null)
		{
			return;
		}

		promptsProgressText.text = string.Format(promptsProgressFormat, currentPrompts, promptsToWin);
	}

	public void SetWordsStorage(int currentWords, int maxWords)
	{
		if (wordsStorageText == null)
		{
			return;
		}

		wordsStorageText.text = string.Format(wordsStorageFormat, currentWords, maxWords);
	}

	public bool IsPromptsPanelOpen()
	{
		return promptsPanel != null && promptsPanel.activeSelf;
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

		if (!isVisible && PromptsControl.Instance != null)
		{
			PromptsControl.Instance.CancelAllUnsubmittedPromptFills();
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

}
