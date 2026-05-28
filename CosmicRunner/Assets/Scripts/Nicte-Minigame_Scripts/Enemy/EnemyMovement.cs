using UnityEngine;

namespace Nicte.Minigame{
// Controla el movimiento vertical del enemigo segun su estado
public class EnemyMovement : MonoBehaviour
{
    // Velocidades por estado y limites de pantalla
    private float normalSpeed =2f;
    private float aggressiveSpeed=4f;
    private float enragedSpeed=6f;
    float direction = 1f;
    private float minY = -3f;
    private float maxY =  2f;
    private float chaseRange    = 8f;
    private float chaseStrength = 0.5f;
    // Referencias al rigidbody y al jugador
    Rigidbody2D rb;
    Transform playerTransform;
    string currentState = "Normal";
    float currentSpeed;

    // Configura el cuerpo fisico del enemigo
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // Busca al jugador y prepara la velocidad base
    void Start()
    {
        currentSpeed = normalSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    // Aplica el movimiento segun el estado actual
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

    // Movimiento base hacia arriba o abajo
    Vector2 MoveNormally()
    {
        return new Vector2(0f, direction * currentSpeed);
    }

    // Movimiento agresivo que intenta acercarse al jugador
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

    // Movimiento enojado que persigue al jugador con mas intensidad
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

    // Ajusta la velocidad del enemigo segun el estado recibido
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
}