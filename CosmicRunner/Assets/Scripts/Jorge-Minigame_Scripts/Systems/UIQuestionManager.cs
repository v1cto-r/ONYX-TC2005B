using UnityEngine;

public class UIQuestionManager : MonoBehaviour
{
    // panel donde se muestra la pregunta
    public GameObject questionPanel;

    // guarda la posicion del checkpoint pendiente
    private Vector3 pendingCheckpoint;

    void Start()
    {
        // al iniciar oculta el panel de preguntas
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    public void ShowQuestion(Vector3 checkpointPos)
    {
        // guarda la posicion del checkpoint tocado
        pendingCheckpoint = checkpointPos;

        // muestra el panel de pregunta
        if (questionPanel != null)
            questionPanel.SetActive(true);

        // pausa el juego mientras responde
        Time.timeScale = 0f;
    }

    public void CorrectAnswer()
    {
        // guarda el checkpoint como respawn
        CheckpointManager.instance.respawnPoint = pendingCheckpoint;

        // oculta el panel y reanuda el juego
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void WrongAnswer()
    {
        // si responde mal pierde una vida
        GameControl.Instance.SpendLives();

        // oculta el panel y reanuda el juego
        if (questionPanel != null)
            questionPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}