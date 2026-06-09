using UnityEngine;

namespace JorgeGame
{
    public class CameraFollow : MonoBehaviour
    {
        // objeto que la camara va a seguir
        [SerializeField] private Transform target;

        // que tan suave sigue la camara al jugador
        [SerializeField] private float smoothTime = 0.3f;

        // distancia fija de la camara respecto al jugador
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        // si esta apagado, la camara no sube ni baja con el jugador
        [SerializeField] private bool followY = true;

        // variable interna usada por smoothdamp
        private Vector3 velocity = Vector3.zero;

        // limites en el eje x para no salir del nivel
        [SerializeField] private float minX;
        [SerializeField] private float maxX;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // posicion a la que la camara quiere llegar
            Vector3 targetPosition = target.position + offset;

            // si no queremos que siga en y, conserva la altura actual de la camara
            if (!followY)
            {
                targetPosition.y = transform.position.y;
            }

            // mantiene la profundidad correcta para juego 2d
            targetPosition.z = offset.z;

            // movimiento suave de la camara
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );

            // limita la camara dentro del nivel
            float clampedX = Mathf.Clamp(smoothedPosition.x, minX, maxX);

            // aplica la posicion final
            transform.position = new Vector3(clampedX, smoothedPosition.y, smoothedPosition.z);
        }
    }
}