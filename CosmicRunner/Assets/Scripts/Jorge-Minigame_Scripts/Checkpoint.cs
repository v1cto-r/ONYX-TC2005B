using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // referencia al manejador de preguntas
    public UIQuestionManager uiManager;

    // variable para evitar que el checkpoint se active mas de una vez
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // se verifica que el objeto que entra sea el jugador
        // y que el checkpoint no haya sido activado antes
        if (collision.CompareTag("Player") && !activated)
        {
            // se marca como activado para que no se repita
            activated = true;

            // se manda la posicion del checkpoint al sistema de preguntas
            // para que si responde bien, se guarde como respawn
            uiManager.ShowQuestion(transform.position);
        }
    }
}