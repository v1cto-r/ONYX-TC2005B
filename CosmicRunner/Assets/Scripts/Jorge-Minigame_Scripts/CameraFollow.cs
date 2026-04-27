using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // referencia al objeto que la camara va a seguir (jugador)
    [SerializeField] private Transform target;

    // se usa para saber hacia donde mira el jugador (izquierda o derecha)
    private SpriteRenderer targetSpriteRenderer;

    // tiempo de suavizado del movimiento de la camara
    [SerializeField] private float smoothTime = 0.3f;

    // desplazamiento de la camara respecto al jugador
    [SerializeField] private Vector3 offset = new Vector3(1f, 0f, -10f);

    // velocidad interna usada por smoothdamp
    private Vector3 velocity = Vector3.zero;

    // limites de movimiento en x
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    void Start()
    {
        // obtiene el spriterenderer del jugador
        targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
    }

    // lateupdate se usa para que la camara se mueva despues del jugador
    private void LateUpdate()
    {
        // detecta hacia donde mira el jugador
        float facing = targetSpriteRenderer.flipX ? -1f : 1f;

        // ajusta el offset dependiendo de la direccion
        Vector3 flippedOffset = new Vector3(offset.x * facing, offset.y, offset.z);

        // calcula la posicion objetivo de la camara
        Vector3 targetPosition = target.position + flippedOffset;

        // suaviza el movimiento de la camara
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // limita la camara en el eje x
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);

        // aplica la posicion final a la camara
        transform.position = new Vector3(clampedX, smoothedPosition.y, smoothedPosition.z);
    }
}