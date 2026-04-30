using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class Goal : MonoBehaviour
{
    // indice de la escena de victoria
    public int victorySceneIndex = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // cuando el jugador toca la meta gana
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(victorySceneIndex);
        }
    }
}
}