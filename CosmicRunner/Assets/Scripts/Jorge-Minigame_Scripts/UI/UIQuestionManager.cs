using UnityEngine;

namespace JorgeGame
{
public class UIQuestionManager : MonoBehaviour
{
    // panel donde se muestra la pregunta
    public GameObject questionPanel;

    // guarda la posicion del checkpoint pendiente
    private Vector3 pendingCheckpoint;

    // referencia al checkpoint actual para poder cambiar su estado
    private Checkpoint currentCheckpoint;

    // panel de la pregunta activa en este checkpoint
    private GameObject currentPanel;

    void Start()
    {
        // al iniciar oculta el panel de preguntas
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    // muestra la pregunta del checkpoint y pausa el juego

public void ShowQuestion(Vector3 checkpointPos, Checkpoint checkpoint, GameObject panel)
{
    pendingCheckpoint = checkpointPos;
    currentCheckpoint = checkpoint;
    currentPanel = panel;

    if (currentPanel != null)
        currentPanel.SetActive(true);

    Time.timeScale = 0f;
}

    // guarda progreso del checkpoint al responder correctamente
    public void CorrectAnswer()
    {

        SFXManager.instance.PlaySFX(SFXManager.instance.checkpointSound, 0.5f);
        // guarda el checkpoint como respawn
        SpawnPoint.instance.respawnPoint = pendingCheckpoint;

        // cambia el color del checkpoint a correcto
        if (currentCheckpoint != null)
            currentCheckpoint.SetCorrect();

        // oculta el panel y reanuda el juego
        if (currentPanel != null)
            currentPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // descuenta vida y marca el checkpoint como incorrecto
    public void WrongAnswer()
    {
        // pierde una vida
        GameControl.Instance.SpendLives();

        // cambia el color del checkpoint a incorrecto
        if (currentCheckpoint != null)
            currentCheckpoint.SetWrong();

        // oculta el panel y reanuda el juego
        if (currentPanel != null)
            currentPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
}