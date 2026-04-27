using UnityEngine;

public class UIQuestionManager : MonoBehaviour
{
    // referencia al panel de la pregunta en el canvas
    public GameObject questionPanel;

    // guarda la posicion del checkpoint pendiente
    // solo se guarda si el jugador responde bien
    private Vector3 pendingCheckpoint;

    void Start()
    {
        // al iniciar el juego se oculta el panel de preguntas
        questionPanel.SetActive(false);
    }

    public void ShowQuestion(Vector3 checkpointPos)
    {
        // se guarda la posicion del checkpoint que se acaba de tocar
        pendingCheckpoint = checkpointPos;

        // se muestra el panel de la pregunta
        questionPanel.SetActive(true);

        // se pausa el juego mientras el jugador responde
        Time.timeScale = 0f;
    }

    public void CorrectAnswer()
    {
        // si responde bien, se guarda el checkpoint como punto de respawn
        CheckpointManager.instance.respawnPoint = pendingCheckpoint;

        // se oculta el panel
        questionPanel.SetActive(false);

        // se reanuda el juego
        Time.timeScale = 1f;
    }

    public void WrongAnswer()
    {
        // si responde mal, pierde una vida
        GameControl.Instance.SpendLives();

        // se oculta el panel
        questionPanel.SetActive(false);

        // se reanuda el juego
        Time.timeScale = 1f;
    }
}