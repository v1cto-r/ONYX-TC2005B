using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class TimerManager : MonoBehaviour
{
    // tiempo inicial en segundos (300 = 5 minutos)
    public float timeRemaining = 300f;

    // texto donde se muestra el tiempo
    public TextMeshProUGUI timerText;

    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        if (timeRemaining > 0)
        {
            // reduce el tiempo cada frame
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            // cuando se acaba el tiempo pasa a derrota
            timeRemaining = 0;
            isRunning = false;

            Time.timeScale = 1f;
            SceneManager.LoadScene(3);
        }
    }

    void UpdateTimerUI()
    {
        // convierte el tiempo a formato minutos y segundos
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
}