using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance;

    [Header("Game Settings")]
    public int promptsToWin = 8;
    public int currentPrompts = 0;
    public int currentScore = 0;

    [Header("UI Control")]
    public float totalGameTime = 120f;
    public float remainingTime;
    public UIControl uiControl;

    
    private bool gameOver;
    private PromptsControl promptsControl;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetReferences();
        StartGame();
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (uiControl != null)
        {
            uiControl.SetTimer(remainingTime);
        }

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

    private void SetReferences()
    {
        if (promptsControl == null)
        {
            promptsControl = PromptsControl.Instance;
        }

        if (uiControl == null)
        {
            uiControl = FindAnyObjectByType<UIControl>();
        }
    }

    public void StartGame()
    {
        currentScore = 0;
        currentPrompts = 0;
        remainingTime = totalGameTime;
        gameOver = false;

        if (uiControl != null)
        {
            uiControl.SetTimer(remainingTime);
            uiControl.SetScore(currentScore);
            uiControl.SetPrompts(currentPrompts, promptsToWin);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        if (uiControl != null)
        {
            uiControl.SetScore(currentScore);
        }
    }

    public void RemoveScore(int amount)
    {
        if (currentScore - amount < 0)
        {
            currentScore = 0;
        }
        else
        {
            currentScore -= amount;
        }

        if (uiControl != null)
        {
            uiControl.SetScore(currentScore);
        }
    }

    public void AddCompletedPrompt()
    {
        currentPrompts += 1;

        if (uiControl != null)
        {
            uiControl.SetPrompts(currentPrompts, promptsToWin);
        }
    }

    private void EndGame()
    {
        gameOver = true;
        SceneManager.LoadScene("EndScene");
    }
}
