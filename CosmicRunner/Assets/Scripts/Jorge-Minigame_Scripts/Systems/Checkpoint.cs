using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // referencia al sistema de preguntas
    public UIQuestionManager uiManager;

    // evita que se active varias veces
    private bool activated = false;

    // renderer del sprite
    private SpriteRenderer sr;
    public GameObject myQuestionPanel;
    // colores
    public Color normalColor = Color.white;
    public Color correctColor = new Color(0.4f, 1f, 0.4f); // verde suave
    public Color wrongColor = new Color(1f, 0.4f, 0.4f);   // rojo suave

    void Start()
    {
        // obtiene el renderer del sprite
        sr = GetComponent<SpriteRenderer>();

        // asegura color normal al inicio
        if (sr != null)
            sr.color = normalColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // solo se activa si lo toca el jugador y no se ha usado antes
        if (collision.CompareTag("Player") && !activated)
        {
            activated = true;

            // muestra la pregunta y guarda la posicion del checkpoint
        uiManager.ShowQuestion(transform.position, this, myQuestionPanel);}
    }

    // se pondra verde cuando responda bien
    public void SetCorrect()
    {
        if (sr != null)
            sr.color = correctColor;
    }

    // se pondra rojo cuando responda mal
    public void SetWrong()
    {
        if (sr != null)
            sr.color = wrongColor;
    }
}