using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax2DSeamless : MonoBehaviour
{
    [Header("Configuración de Referencia")]
    [SerializeField] private Transform camaraTarget; // Asigna aquí la cámara principal (u objeto que la cámara sigue)

    [Header("Settings de Efecto")]
    [Tooltip("0 = estático, 1 = se mueve igual que la cámara. Bajos (0.1-0.3) para fondos lejanos, altos (0.5-0.8) para cercanos.")]
    [SerializeField] private Vector2 factorParalax = new Vector2(0.3f, 0.3f); // Separado X/Y

    [Tooltip("Tamaño de la 'ventana' en el centro donde la cámara se mueve SIN afectar el fondo.")]
    [SerializeField] private Vector2 zonaMuertaDimensiones = new Vector2(3f, 3f); // El radio de la ventana (ancho/alto)

    // Variables internas de estado
    private Vector2 posicionInicialCuerpo; // Posición de este objeto en el mundo al Start
    private Vector2 longitudSprite;        // Medidas del Sprite para el looping infinito
    private Vector2 centroVentanaActual;   // El centro de la 'no-parallax window'

    void Start()
    {
        // 1. Validaciones básicas
        if (camaraTarget == null)
        {
            camaraTarget = Camera.main.transform;
            if (camaraTarget == null) { Debug.LogError("No hay cámara objetivo."); enabled = false; return; }
        }

        // 2. Guardar estado inicial y medidas
        posicionInicialCuerpo = transform.position;
        longitudSprite = GetComponent<SpriteRenderer>().bounds.size;

        // 3. Inicializar ventana centrada en la cámara inicial
        centroVentanaActual = camaraTarget.position;
    }

    // Usamos LateUpdate para asegurar que la cámara ya se movió por su propio script de seguimiento.
    void LateUpdate()
    {
        // --- PARTE 1: Gestión de la Ventana ('No Paralax Window') ---
        // Calculamos la distancia entre la cámara y el centro de nuestra ventana actual
        Vector2 offsetDesdeCentro = (Vector2)camaraTarget.position - centroVentanaActual;

        // Comprobamos si la cámara ha salido de los límites X de la ventana
        if (Mathf.Abs(offsetDesdeCentro.x) > zonaMuertaDimensiones.x)
        {
            // Movemos el centro de la ventana en X para 'seguir' a la cámara de forma rígida
            // Usamos Mathf.Sign para saber si salió a la derecha (+) o izquierda (-)
            float correccionX = offsetDesdeCentro.x - (zonaMuertaDimensiones.x * Mathf.Sign(offsetDesdeCentro.x));
            centroVentanaActual.x += correccionX;
        }

        // Comprobamos si la cámara ha salido de los límites Y de la ventana
        if (Mathf.Abs(offsetDesdeCentro.y) > zonaMuertaDimensiones.y)
        {
            // Hacemos lo mismo en Y
            float correccionY = offsetDesdeCentro.y - (zonaMuertaDimensiones.y * Mathf.Sign(offsetDesdeCentro.y));
            centroVentanaActual.y += correccionY;
        }


        // --- PARTE 2: Cálculo del Efecto Paralax (2D Libre) ---
        // Ahora calculamos cuánto se ha movido el CENTRO DE LA VENTANA relativo a su inicio.
        // Este movimiento suavizado por la ventana es el que aplica el paralax.
        
        Vector2 movimientoCamaSuavizadoX = new Vector2(
            centroVentanaActual.x - posicionInicialCuerpo.x, 
            centroVentanaActual.y - posicionInicialCuerpo.y);

        // Calculamos la distancia de paralax real a aplicar
        Vector2 distParalax = new Vector2(
            movimientoCamaSuavizadoX.x * factorParalax.x,
            movimientoCamaSuavizadoX.y * factorParalax.y
        );

        // Actualizamos posición manteniendo Z original para capas
        transform.position = new Vector3(posicionInicialCuerpo.x + distParalax.x, posicionInicialCuerpo.y + distParalax.y, transform.position.z);


        // --- PARTE 3: Lógica de Infinite Scrolling (Adaptada para 2D) ---
        // Esta parte es la que 'teletransporta' el fondo cuando el centro de la cámara
        // se aleja demasiado del centro de nuestro tile sprite.

        // Calculamos 'tempX/Y' (la posición teórica 'estática' relativa a la cámara del script platformer)
        // Adaptamos la lógica de tu ejemplo para 2D.
        float tempX = camaraTarget.position.x * (1 - factorParalax.x);
        float tempY = camaraTarget.position.y * (1 - factorParalax.y);

        // Chequeo de Loop Infinito en X
        if (tempX > posicionInicialCuerpo.x + longitudSprite.x / 2f)
            posicionInicialCuerpo.x += longitudSprite.x;
        else if (tempX < posicionInicialCuerpo.x - longitudSprite.x / 2f)
            posicionInicialCuerpo.x -= longitudSprite.x;

        // Chequeo de Loop Infinito en Y
        if (tempY > posicionInicialCuerpo.x + longitudSprite.y / 2f)
            posicionInicialCuerpo.y += longitudSprite.y;
        else if (tempY < posicionInicialCuerpo.x - longitudSprite.y / 2f)
            posicionInicialCuerpo.y -= longitudSprite.y;
    }

    // Opcional: Para visualizar la ventana en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(centroVentanaActual, new Vector3(zonaMuertaDimensiones.x * 2f, zonaMuertaDimensiones.y * 2f, 0.1f));
        }
    }
}