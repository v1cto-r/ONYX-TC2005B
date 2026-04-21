using UnityEngine;
using TMPro;

public class TimerControl : MonoBehaviour
{
    // Referencia al texto que muestra el tiempo transcurrido
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private float remainingTime = 120f; // Tiempo inicial en segundos

    // Variable para almacenar el tiempo transcurrido

    // Update es llamado una vez por frame
    void Update()
    {
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
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
