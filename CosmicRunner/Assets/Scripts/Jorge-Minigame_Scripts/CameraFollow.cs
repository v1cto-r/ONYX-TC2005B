using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Referencia al objetivo que la camara seguira (el jugador) y obtiene su SpriteRenderer para determinar su direccion
    [SerializeField] private Transform target;
    private SpriteRenderer targetSpriteRenderer;
    
    // Tiempo de suavizado para el movimiento de la camara
    [SerializeField] private float smoothTime = 0.3f;
    
    // Desplazamiento de la camara con respecto al objetivo
    [SerializeField] private Vector3 offset = new Vector3(1f, 0f, -10f);

    // Velocidad actual de la camara
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    // Start es llamado antes del primer frame update
    void Start()
    {
        // Obtener la referencia al SpriteRenderer del objetivo
        targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
    }

    // LateUpdate es para seguir al jugador despues de que se haya movido
    private void LateUpdate()
    {
        // Calcular la direccion del objetivo para voltear la camara
        float facing = targetSpriteRenderer.flipX ? -1f : 1f;
        Vector3 flippedOffset = new Vector3(offset.x * facing, offset.y, offset.z);

        // Calcular la posicion objetivo de la camara con el desplazamiento y suavizar el movimiento
        Vector3 targetPosition = target.position + flippedOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Limitar SOLO en X
        float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);

        transform.position = new Vector3(clampedX, smoothedPosition.y, smoothedPosition.z);
    }
}