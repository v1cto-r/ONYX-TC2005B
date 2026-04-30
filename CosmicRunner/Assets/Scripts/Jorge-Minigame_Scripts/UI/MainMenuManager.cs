using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class MainMenuManager : MonoBehaviour
{
    // carga la escena principal del minijuego
    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }
}
}