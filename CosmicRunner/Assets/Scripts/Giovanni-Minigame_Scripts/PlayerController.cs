using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SpaceshipController2D : MonoBehaviour
{
    [Header("Motores Principales")]
    public float fuerzaEmpuje = 15f;
    public float velocidadRotacion = 250f; 
    [Tooltip("Velocidad máxima que puede alcanzar la nave")]
    public float velocidadMaxima = 1f; // <-- NUEVA VARIABLE

    [Header("Sistema de Control de Vuelo")]
    [Tooltip("Activa los propulsores RCS para frenar automáticamente cuando no hay input")]
    public bool amortiguadoresActivados = true;
    [Tooltip("Fuerza con la que la nave contrarresta la inercia (Propulsores retro)")]
    public float fuerzaFrenado = 10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; // El entorno sigue siendo un vacío perfecto
        rb.angularDamping = 3f;
    }

    void FixedUpdate()
    {
        Vector2 vectorDeseado = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) vectorDeseado.y += 1;
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) vectorDeseado.y -= 1;
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) vectorDeseado.x += 1;
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) vectorDeseado.x -= 1;
        }

        vectorDeseado = vectorDeseado.normalized;

        // ESTADO 1: Piloto manual (Acelerando)
        if (vectorDeseado != Vector2.zero)
        {
            // Rotación
            float anguloDeseado = Mathf.Atan2(vectorDeseado.y, vectorDeseado.x) * Mathf.Rad2Deg - 90f;
            float anguloActual = rb.rotation;
            float nuevoAngulo = Mathf.MoveTowardsAngle(anguloActual, anguloDeseado, velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(nuevoAngulo);

            // Empuje
            float alineacion = Vector2.Dot(transform.up, vectorDeseado);

            if (alineacion > 0.5f)
            {
                rb.AddForce(transform.up * fuerzaEmpuje * alineacion, ForceMode2D.Force);
            }
        }
        // ESTADO 2: Piloto automático (Frenado)
        else if (amortiguadoresActivados)
        {
            // Si la nave se está moviendo, actuamos para llevar la velocidad a cero
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                // Calculamos el vector opuesto a nuestro movimiento actual
                Vector2 direccionFrenado = -rb.linearVelocity.normalized;
                
                // Aplicamos la fuerza. 
                // Usamos Mathf.Min para evitar que una fuerza de frenado muy alta 
                // nos empuje hacia atrás si ya estamos casi detenidos (evita oscilaciones).
                float fuerzaAplicada = Mathf.Min(fuerzaFrenado, rb.linearVelocity.magnitude / Time.fixedDeltaTime);
                
                rb.AddForce(direccionFrenado * fuerzaAplicada, ForceMode2D.Force);
            }
            else
            {
                // Zona muerta: si la velocidad es mínima, "apagamos" el movimiento para evitar micro-temblores
                rb.linearVelocity = Vector2.zero;
            }
        }

        // --- LÍMITE DE VELOCIDAD TERMINAL (NUEVO) ---
        // Se coloca al final para garantizar que actúe sobre todas las fuerzas aplicadas en este frame
        if (rb.linearVelocity.magnitude > velocidadMaxima)
        {
            // Mantenemos la dirección (rb.velocity.normalized) pero le asignamos la velocidad máxima permitida
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadMaxima;
        }
    }
}