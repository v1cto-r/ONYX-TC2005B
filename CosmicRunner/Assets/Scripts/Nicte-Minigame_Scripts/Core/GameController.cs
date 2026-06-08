using UnityEngine;
using UnityEngine.SceneManagement;
namespace Nicte.Minigame{
public enum GameState
{
    Playing,Victory,Defeat
}
public class GameController : MonoBehaviour
{
    public static GameController instancia;
    public GameObject startScreen;
    string mainMenuScene="AtaqueEstelarGameStart";
    string gameScene="AtaqueEstelarGame";
    string DefeatScene="AtaqueEstelarDefeat";
    string VictoryScene="AtaqueEstelarVictory";
    public float time=240f;
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
        ChangeState("Victory");
    }

    public void SetDefeat()
    {
        ChangeState("Defeat");
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

    
}
}