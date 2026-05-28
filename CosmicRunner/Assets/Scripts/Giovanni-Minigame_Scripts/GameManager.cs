using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para cambiar entre escenas

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Sistema de Vidas")]
    [Tooltip("Debe coincidir con la cantidad de barritas en tu UI")]
    public int vidasMaximas = 6; 
    private int vidasActuales;
    [Tooltip("Arrastra aquí los objetos 'Charge' de las vidas en orden (de izquierda a derecha)")]
    public GameObject[] barrasDeVidaUI;

    [Header("Temporizador")]
    public float tiempoDePartidaMinutos = 3f;
    private float tiempoRestante;
    public TextMeshProUGUI textoTemporizador; // El texto en la parte superior central

    [Header("Navegación de Escenas")]
    [Tooltip("Escribe los nombres exactos de tus escenas")]
    public string escenaVictoria = "PantallaGanar";
    public string escenaDerrota = "PantallaPerder";

    private bool juegoTerminado = false;
    private PlayerController jugadorRef;

    private void Awake()
    {
        // Patrón Singleton para acceder al GameManager desde cualquier script
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Asegurarnos de que el tiempo corra normal al iniciar la escena
        Time.timeScale = 1f; 
        
        vidasActuales = vidasMaximas;
        tiempoRestante = tiempoDePartidaMinutos * 60f; // Convertir a segundos
        
        jugadorRef = FindObjectOfType<PlayerController>();
        
        ActualizarUIVidas();
    }

    private void Update()
    {
        if (juegoTerminado) return;

        ManejarTemporizador();
    }

    private void ManejarTemporizador()
    {
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            ActualizarTextoTemporizador();
            
            // Si el tiempo se acaba y aún tenemos vidas, GANAMOS
            if (vidasActuales > 0)
            {
                TerminarPartida(true); 
            }
        }
        else
        {
            ActualizarTextoTemporizador();
        }
    }

    private void ActualizarTextoTemporizador()
    {
        if (textoTemporizador != null)
        {
            // Calcular minutos y segundos restantes
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            
            // Formatear el texto para que siempre muestre dos dígitos (ej. 03:09)
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    // Método público que los enemigos y obstáculos llamarán al chocar
    public void RecibirDano(int cantidad)
    {
        if (juegoTerminado) return;

        // Si el jugador recogió el power-up de defensa, ignoramos el daño
        if (jugadorRef != null && jugadorRef.esInmune)
        {
            Debug.Log("Daño bloqueado: Escudo activo.");
            return;
        }

        vidasActuales -= cantidad;
        if (vidasActuales < 0) vidasActuales = 0;

        ActualizarUIVidas();

        // Condición de derrota: 0 vidas
        if (vidasActuales <= 0)
        {
            TerminarPartida(false); 
        }
    }

    private void ActualizarUIVidas()
    {
        // Apaga o enciende los sprites según la vida actual
        for (int i = 0; i < barrasDeVidaUI.Length; i++)
        {
            // Si i (índice) es menor que vidasActuales, se enciende. Si no, se apaga.
            barrasDeVidaUI[i].SetActive(i < vidasActuales);
        }
    }

    private void TerminarPartida(bool victoria)
    {
        juegoTerminado = true;
        
        // Congelamos las físicas y el tiempo del juego para una transición limpia
        Time.timeScale = 0f; 

        if (victoria)
        {
            Debug.Log("¡Tiempo cumplido! Cargando pantalla de victoria...");
            SceneManager.LoadScene(escenaVictoria);
        }
        else
        {
            Debug.Log("0 vidas. Cargando pantalla de derrota...");
            SceneManager.LoadScene(escenaDerrota);
        }
    }
}