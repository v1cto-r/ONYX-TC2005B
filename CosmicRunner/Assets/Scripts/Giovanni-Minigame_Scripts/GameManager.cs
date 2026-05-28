using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para cambiar entre escenas
namespace Gio.Minigame{
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int vidasMaximas = 6; 
    private int vidasActuales;

    public GameObject[] barrasDeVidaUI;


    public float tiempoDePartidaMinutos = 3f;
    private float tiempoRestante;
    public TextMeshProUGUI textoTemporizador; 
    private bool juegoTerminado = false;
    private PlayerController jugadorRef;


    public TextMeshProUGUI textoCreditosUI; 
    private int creditosActuales = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f; 
        
        vidasActuales = vidasMaximas;
        tiempoRestante = tiempoDePartidaMinutos * 60f; 
        
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
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (juegoTerminado) return;

        if (jugadorRef != null && jugadorRef.esInmune)
        {
            Debug.Log("Daño bloqueado: Escudo activo.");
            return;
        }

        vidasActuales -= cantidad;
        if (vidasActuales < 0) vidasActuales = 0;

        ActualizarUIVidas();

        if (vidasActuales <= 0)
        {
            TerminarPartida(false); 
        }
    }

    private void ActualizarUIVidas()
    {
        for (int i = 0; i < barrasDeVidaUI.Length; i++)
        {
            barrasDeVidaUI[i].SetActive(i < vidasActuales);
        }
    }

    public void SumarMonedas(int cantidad)
    {
        creditosActuales += cantidad;
        
        if (textoCreditosUI != null)
        {
            textoCreditosUI.text = creditosActuales.ToString();
        }
    }

    private void TerminarPartida(bool victoria)
    {
        juegoTerminado = true;

        PlayerPrefs.SetInt("resultado", victoria ? 1 : 0);
        PlayerPrefs.SetInt("credits", creditosActuales);
        PlayerPrefs.Save();

        SceneManager.LoadScene("IEEndScene"); 
    }
}
}