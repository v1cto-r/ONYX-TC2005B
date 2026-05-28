using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para cambiar entre escenas
using UnityEngine.InputSystem;
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


    public GameObject panelPausa;
    private bool juegoPausado = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // --- FIX CRÍTICO: Protección contra Nulos en Sonido ---
        if (SFXManager.Instance != null && SFXManager.Instance.musicaFondo1 != null) 
        {
            SFXManager.Instance.PlayBackgroundMusic(SFXManager.Instance.musicaFondo1); 
        }

        Time.timeScale = 1f; 
        
        vidasActuales = vidasMaximas;
        tiempoRestante = tiempoDePartidaMinutos * 60f; 
        
        jugadorRef = FindObjectOfType<PlayerController>();
        
        ActualizarUIVidas();
    }

    private void Update()
    {
        if (juegoTerminado) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            AlternarPausa();
        }
        if (!juegoPausado)
        {
            ManejarTemporizador();
        }
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

        if (SFXManager.Instance != null) SFXManager.Instance.PlayHitSound();

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
        if (SFXManager.Instance != null) SFXManager.Instance.StopBackgroundMusic();

        PlayerPrefs.SetInt("resultado", victoria ? 1 : 0);
        PlayerPrefs.SetInt("credits", creditosActuales);
        PlayerPrefs.Save();

        SceneManager.LoadScene("IEEndScene"); 
    }

    public void AlternarPausa()
    {
        juegoPausado = !juegoPausado;

        if (juegoPausado)
        {
            Time.timeScale = 0f;
            if (panelPausa != null) panelPausa.SetActive(true);
            if (jugadorRef != null) jugadorRef.enabled = false; 
        }
        else
        {
            Time.timeScale = 1f;
            if (panelPausa != null) panelPausa.SetActive(false);
            if (jugadorRef != null) jugadorRef.enabled = true;
        }
    }
    public void ReanudarJuego()
    {
        if (juegoPausado)
        {
            AlternarPausa();
        }
    }
}
}