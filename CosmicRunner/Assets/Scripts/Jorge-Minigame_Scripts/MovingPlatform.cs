using UnityEngine;

namespace JorgeGame
{
public class MovingPlatform : MonoBehaviour
{
    // puntos entre los que se mueve la plataforma
    public Transform pointA;
    public Transform pointB;

    // velocidad de movimiento
    public float moveSpeed = 2f;

    private Vector3 nextPosition;

    void Start()
    {
        // empieza moviendose hacia el punto B
        nextPosition = pointB.position;
    }

    void Update()
    {
        // mueve la plataforma entre los dos puntos
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPosition,
            moveSpeed * Time.deltaTime
        );

        // cuando llega a un punto cambia al otro
        if (Vector2.Distance(transform.position, nextPosition) < 0.05f)
        {
            nextPosition = (nextPosition == pointA.position)
                ? pointB.position
                : pointA.position;
        }
    }
}
}