using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Requerido para interactuar con la barra de la UI

[RequireComponent(typeof(Rigidbody2D))]
public class SpaceshipController2D : MonoBehaviour
{
    [Header("Motores Principales")]
    public float fuerzaEmpuje = 15f;
    public float velocidadRotacion = 250f; 
    [Tooltip("Velocidad máxima que puede alcanzar la nave")]
    public float velocidadMaxima = 5f;

    [Header("Sistema de Control de Vuelo")]
    public bool amortiguadoresActivados = true;
    public float fuerzaFrenado = 10f;

    [Header("Ofensiva (Paso 2.1)")]
    public GameObject prefabProyectil;
    public Transform puntoDisparo;
    public float velocidadProyectil = 12f;
    public bool heredarVelocidadNave = true;

    [Header("Sobrecalentamiento (Paso 2.2)")]
    [Tooltip("Arrastra aquí el Slider de sobrecalentamiento del Canvas")]
    public Slider barraSobrecalentamiento;
    public float sobrecalentamientoMaximo = 100f;
    public float costoPorDisparo = 15f;
    public float tasaEnfriamiento = 20f; // Cuánto sobrecalentamiento baja por segundo

    private Rigidbody2D rb;
    private float sobrecalentamientoActual = 0f;
    private bool armaBloqueada = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; 
        rb.angularDamping = 3f;

        // Inicializar la barra de la UI si está asignada
        if (barraSobrecalentamiento != null)
        {
            barraSobrecalentamiento.maxValue = sobrecalentamientoMaximo;
            barraSobrecalentamiento.value = 0f;
        }
    }

    void Update()
    {
        ManejarDisparo();
        ManejarEnfriamiento();
        ActualizarUI();
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
            float anguloDeseado = Mathf.Atan2(vectorDeseado.y, vectorDeseado.x) * Mathf.Rad2Deg - 90f;
            float anguloActual = rb.rotation;
            float nuevoAngulo = Mathf.MoveTowardsAngle(anguloActual, anguloDeseado, velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(nuevoAngulo);

            float alineacion = Vector2.Dot(transform.up, vectorDeseado);

            if (alineacion > 0.5f)
            {
                rb.AddForce(transform.up * fuerzaEmpuje * alineacion, ForceMode2D.Force);
            }
        }
        // ESTADO 2: Piloto automático (Frenado)
        else if (amortiguadoresActivados)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                Vector2 direccionFrenado = -rb.linearVelocity.normalized;
                float fuerzaAplicada = Mathf.Min(fuerzaFrenado, rb.linearVelocity.magnitude / Time.fixedDeltaTime);
                rb.AddForce(direccionFrenado * fuerzaAplicada, ForceMode2D.Force);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // LÍMITE DE VELOCIDAD TERMINAL
        if (rb.linearVelocity.magnitude > velocidadMaxima)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * velocidadMaxima;
        }
    }

    private void ManejarDisparo()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Solo permite disparar si el arma no está bloqueada por sobrecalentamiento
            if (!armaBloqueada)
            {
                EjecutarDisparo();
            }
        }
    }

    private void EjecutarDisparo()
    {
        if (prefabProyectil == null || puntoDisparo == null) return;

        // 1. Instanciación (Paso 2.1)
        GameObject nuevoProyectil = Instantiate(prefabProyectil, puntoDisparo.position, puntoDisparo.rotation);
        Rigidbody2D rbProyectil = nuevoProyectil.GetComponent<Rigidbody2D>();

        if (rbProyectil != null)
        {
            Vector2 velocidadInicial = transform.up * velocidadProyectil;
            if (heredarVelocidadNave)
            {
                velocidadInicial += rb.linearVelocity;
            }
            rbProyectil.linearVelocity = velocidadInicial;
        }

        // 2. Control de Sobrecalentamiento (Paso 2.2)
        sobrecalentamientoActual += costoPorDisparo;

        if (sobrecalentamientoActual >= sobrecalentamientoMaximo)
        {
            sobrecalentamientoActual = sobrecalentamientoMaximo;
            armaBloqueada = true; // Bloqueo activado
        }
    }

    private void ManejarEnfriamiento()
    {
        if (sobrecalentamientoActual > 0f)
        {
            sobrecalentamientoActual -= tasaEnfriamiento * Time.deltaTime;

            // Condición estricta: Solo se desbloquea si baja a cero en su totalidad
            if (sobrecalentamientoActual <= 0f)
            {
                sobrecalentamientoActual = 0f;
                armaBloqueada = false; // Bloqueo desactivado
            }
        }
    }

    private void ActualizarUI()
    {
        if (barraSobrecalentamiento != null)
        {
            barraSobrecalentamiento.value = sobrecalentamientoActual;
        }
    }
}