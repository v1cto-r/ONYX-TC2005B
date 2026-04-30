using UnityEngine;

namespace JorgeGame
{
public class CameraFollow : MonoBehaviour
{
    // objeto que la camara va a seguir
    [SerializeField] private Transform target;

    // para saber hacia donde mira el jugador
    private SpriteRenderer targetSpriteRenderer;

    // que tan suave sigue la camara al jugador
    [SerializeField] private float smoothTime = 0.3f;

    // distancia de la camara respecto al jugador
    [SerializeField] private Vector3 offset = new Vector3(1f, 0f, -10f);

    // variable interna usada por smoothdamp
    private Vector3 velocity = Vector3.zero;

    // limites en el eje x para no salir del nivel
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    void Start()
    {
        // obtiene el sprite del jugador para saber su direccion
        targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // detecta si el jugador mira izquierda o derecha
        float facing = targetSpriteRenderer.flipX ? -1f : 1f;

        // invierte el offset segun la direccion
        Vector3 flippedOffset = new Vector3(offset.x * facing, offset.y, offset.z);

        // posicion a la que la camara quiere llegar
        Vector3 targetPosition = target.position + flippedOffset;

        // movimiento suave de la camara
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // limita la camara dentro del nivel
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);

        // aplica la posicion final
        transform.position = new Vector3(clampedX, smoothedPosition.y, smoothedPosition.z);
    }
}
}