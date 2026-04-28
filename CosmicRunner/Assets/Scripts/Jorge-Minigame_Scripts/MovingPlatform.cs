using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;

    void Start()
    {
        nextPosition = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPosition,
            moveSpeed * Time.deltaTime
        );

        // usar distancia en lugar de ==
        if (Vector2.Distance(transform.position, nextPosition) < 0.05f)
        {
            nextPosition = (nextPosition == pointA.position)
                ? pointB.position
                : pointA.position;
        }
    }
}