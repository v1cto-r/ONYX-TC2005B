using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float normalSpeed =2f;
    private float aggressiveSpeed=4f;
    private float enragedSpeed=6f;
    float direction = 1f;
    private float minY = -3f;
    private float maxY =  2f;
    private float chaseRange    = 8f;
    private float chaseStrength = 0.6f;
    Rigidbody2D rb;
    Transform playerTransform;
    string currentState = "Normal";
    float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        currentSpeed = normalSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void FixedUpdate()
    {
        Vector2 velocity = Vector2.zero;

        if (currentState == "Normal")
        {
            velocity = MoveNormally();
        }
        else if (currentState == "Aggressive")
        {
            velocity = MoveAggressive();
        }
        else if (currentState == "Enraged")
        {
            velocity = MoveEnraged();
        }

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, velocity, Time.fixedDeltaTime * 5f);

        Vector3 pos = transform.position;
        if (pos.y >= maxY)
        {
            direction = -1f;
        }
        else if (pos.y <= minY)
        {
            direction = 1f;
        }

        if (pos.y > maxY)
        {
            pos.y = maxY;
        }
        else if (pos.y < minY)
        {
            pos.y = minY;
        }

        transform.position = pos;
    }

    Vector2 MoveNormally()
    {
        return new Vector2(0f, direction * currentSpeed);
    }

    Vector2 MoveAggressive()
    {
        Vector2 baseVelocity = MoveNormally();

        if (playerTransform != null)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist < chaseRange)
            {
                float deltaY = playerTransform.position.y - transform.position.y;
                if (Mathf.Abs(deltaY) > 0.1f)
                {
                    float dirY = Mathf.Sign(deltaY);
                    Vector2 target = new Vector2(0f, dirY * currentSpeed);

                    return Vector2.Lerp(baseVelocity, target, chaseStrength);
                }
            }
        }

        return baseVelocity;
    }

    Vector2 MoveEnraged()
    {
        if (playerTransform == null) return MoveAggressive();

        float deltaY = playerTransform.position.y - transform.position.y;

        if (Mathf.Abs(deltaY) < 0.1f)
        {
            return Vector2.zero;
        }

        float dirY = Mathf.Sign(deltaY);
        return new Vector2(0f, dirY * currentSpeed);
    }

    public void StateChanged(string state)
    {
        currentState = state;

        if (state == "Normal")
        {         
            currentSpeed = normalSpeed;
        }
        else if (state == "Aggressive")
        {
            currentSpeed = aggressiveSpeed;
        }
        else if (state == "Enraged")
        {
            currentSpeed = enragedSpeed;
        }
    }
}
