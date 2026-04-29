using UnityEngine;

public class UIQuestionManager : MonoBehaviour
{
    // panel donde se muestra la pregunta
    public GameObject questionPanel;

    // guarda la posicion del checkpoint pendiente
    private Vector3 pendingCheckpoint;

    // referencia al checkpoint actual para poder cambiar su estado
    private Checkpoint currentCheckpoint;

    void Start()
    {
        // al iniciar oculta el panel de preguntas
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    // muestra la pregunta y guarda el checkpoint que la activo
    public void ShowQuestion(Vector3 checkpointPos, Checkpoint checkpoint)
    {
        // guarda la posicion del checkpoint
        pendingCheckpoint = checkpointPos;

        // guarda referencia del checkpoint actual
        currentCheckpoint = checkpoint;

        // muestra el panel
        if (questionPanel != null)
            questionPanel.SetActive(true);

        // pausa el juego mientras responde
        Time.timeScale = 0f;
    }

    public void CorrectAnswer()
    {
        // guarda el checkpoint como respawn
        CheckpointManager.instance.respawnPoint = pendingCheckpoint;

        // cambia el color del checkpoint a correcto
        if (currentCheckpoint != null)
            currentCheckpoint.SetCorrect();

        // oculta el panel y reanuda el juego
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void WrongAnswer()
    {
        // pierde una vida
        GameControl.Instance.SpendLives();

        // cambia el color del checkpoint a incorrecto
        if (currentCheckpoint != null)
            currentCheckpoint.SetWrong();

        // oculta el panel y reanuda el juego
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}