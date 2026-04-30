using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }
}
}