using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,Victory,Defeat
}
public class GameController : MonoBehaviour
{
    private static GameController instancia;
    public string mainMenuScene="MainMenu";
    public string gameScene="Game";
    public string resultScene="Result";
    public float time=300f;
    public float timeRemaining;
    public GameState currentState;

    void Awake()
    {
        if (instancia==null)
        {
            instancia=this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timeRemaining=time;
        currentState=GameState.Playing;
    }

    void Update()
    {
        if (currentState!=GameState.Playing)
        {
            return;
        }

        timeRemaining-=Time.deltaTime;

        if (timeRemaining<=0f)
        {
            timeRemaining=0f;
            SetDefeat();
        }
        Debug.Log("Tiempo restante: "+timeRemaining);
    }

    public void SetVictory()
    {
        currentState=GameState.Victory;
        Debug.Log("Victoria");
        EndGame();
    }

    public void SetDefeat()
    {
        currentState=GameState.Defeat;
        Debug.Log("Derrota");
        EndGame();
    }

    void EndGame()
    {
        Time.timeScale=0f;
        Invoke("GoToResult", 2f);
    }

    public void GoToMainMenu()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void GoToGame()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(gameScene);
    }

    public void GoToResult()
    {
        SceneManager.LoadScene(resultScene);
    }

    public void Pause()
    {
        if (currentState!=GameState.Playing)
            return;

        if (Time.timeScale==0f)
        {
            Time.timeScale=1f;
        }
        else
        {
            Time.timeScale=0f;
        }
    }
}