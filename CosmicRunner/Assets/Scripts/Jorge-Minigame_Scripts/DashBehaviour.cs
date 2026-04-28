using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashBehaviour : MonoBehaviour
{
    // Variable declarada para almacenar la accion de dash del sistema de entrada
    private InputAction dashAction;
    
    // Rigidbody2D del jugador para aplicar la velocidad de dash
    private Rigidbody2D rb;

    // Variables para controlar el dash
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashDuration = 0.12f;
    
    // Variable para almacenar la direccion del dash basada en la entrada del jugador
    private Vector2 dashDirection;
    
    // Variables para controlar el estado del dash
    private bool isDashing = false;
    private bool canDash = true;
    
    // TrailRenderer para el efecto visual del dash
    private TrailRenderer trailRenderer;
    
    // Variable para almacenar la gravedad original del Rigidbody2D
    private float originalGravityScale;

    // Control de sonidos
  

    // Start es llamado una vez antes de la primera ejecucion
    void Start()
    {
        // Obtener el componente Rigidbody2D y TrailRenderer del jugador
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        // Encontrar la accion de dash en el sistema de entrada
        dashAction = InputSystem.actions.FindAction("Sprint");
        
        // Almacenar la gravedad original del Rigidbody2D para restaurarla despues del dash
        originalGravityScale = rb.gravityScale;

        // Obtener la referencia al SFXManager
   
    }

    // Update is called once per frame
    void Update()
    {
        // Evitar que el dash se active mientras el juego esta pausado
        if (Time.timeScale == 0) {
            return;
        }
        // Verificar si la accion de dash ha sido activada y el jugador puede dashear
        else if (dashAction.WasPressedThisFrame() && canDash)
        {
            // Modificacion del estado del dash
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;

            // Obtener la direccion del dash basada en la entrada del jugador
            Vector2 moveInput = GetComponent<PlayerInput>().actions["Move"].ReadValue<Vector2>();

            // Propulsor: siempre hacia arriba + poquito a los lados
                dashDirection = new Vector2(moveInput.x * 0.2f, 1f).normalized;
                 rb.gravityScale = 0f;

            // Reproducir el sonido de dash
 

            // Iniciar la corrutina para detener el dash despues de la duracion especificada
            StartCoroutine(StopDash());

        }

        // Si el jugador esta dasheando, aplicar la velocidad de dash
        if (isDashing)
        {
            // Aplicar la velocidad de dash al Rigidbody2D del jugador
            rb.linearVelocity = dashDirection * dashSpeed;

            return;
        }
    }

    // Corrutina para detener el dash despues de la duracion especificada
    private IEnumerator StopDash()
    {
        // Esperar la duracion del dash antes de detenerlo
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        trailRenderer.emitting = false;
        
        // Restaurar la gravedad original del Rigidbody2D despues de detener el dash
        rb.gravityScale = originalGravityScale;
    }

    // Metodo para detectar colisiones con el suelo y permitir que el jugador pueda dashear nuevamente
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si el jugador ha colisionado con el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            canDash = true;
        }
    }
}