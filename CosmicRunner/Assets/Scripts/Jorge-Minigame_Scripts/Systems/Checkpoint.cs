using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // referencia al sistema de preguntas
    public UIQuestionManager uiManager;

    // evita que se active varias veces
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // solo se activa si lo toca el jugador y no se ha usado antes
        if (collision.CompareTag("Player") && !activated)
        {
            activated = true;

            // muestra la pregunta y guarda la posicion del checkpoint
            uiManager.ShowQuestion(transform.position);
        }
    }
}