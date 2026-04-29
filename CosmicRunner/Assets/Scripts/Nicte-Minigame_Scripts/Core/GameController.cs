using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,Victory,Defeat
}
public class GameController : MonoBehaviour
{
    public static GameController instancia;
    public string mainMenuScene="MainMenu";
    public string gameScene="Game";
    public string DefeatScene="Defeat";
    public string VictoryScene="Victory";
    public float time=300f;
    public float timeRemaining;
    public string currentState = "Playing";

    void Awake()
    {
        if (instancia==null)
        {
            instancia=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timeRemaining=time;
        currentState="Playing";
        Time.timeScale=1f;
    }

    void Update()
    {
        if (currentState!="Playing")
        {
            return;
        }

        timeRemaining-=Time.deltaTime;

        if (timeRemaining<=0f)
        {
            timeRemaining=0f;
            SetDefeat();
        }
    }

    public void ChangeState(string newState)
    {
        if (currentState == newState) return;
 
        currentState = newState;
    
 
        if (newState == "Victory")
        {
            WinGame();
        }else if (newState == "Defeat")
        {
            LoseGame();
        }
        else if (newState == "Playing")
        {
            Time.timeScale = 1f;
        }
    }

    public void SetVictory()
    {
        currentState="Victory";
        ChangeState("Victory");
        Debug.Log("Victoria");
        WinGame();
    }

    public void SetDefeat()
    {
        currentState="Defeat";
        ChangeState("Defeat");
        Debug.Log("Derrota");
        LoseGame();
    }

    void LoseGame()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(DefeatScene);
    }

    void WinGame()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(VictoryScene);
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

    public void Pause()
    {
        if (currentState!="Playing")
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