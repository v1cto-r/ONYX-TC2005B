using UnityEngine;
using UnityEngine.SceneManagement;

namespace JorgeGame
{
public class GameControl : MonoBehaviour
{
    // instancia global para acceder desde otros scripts
    public static GameControl Instance;

    // vidas iniciales del jugador
    public int initialLives = 5;

    // referencia a la interfaz (corazones y monedas)
    public UIControler uiControler;

    // contador de monedas
    public int coins = 0;

    void Start()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayMusic();
        }
    }

    void Awake()
    {
        // guarda la instancia unica
        Instance = this;

        // reinicia las vidas al empezar el juego
        PlayerPrefs.SetInt("Lives", initialLives);

        // busca el ui si no esta asignado
        if (uiControler == null)
            uiControler = FindAnyObjectByType<UIControler>();

        // evita que se destruya al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
    }

    // devuelve las vidas actuales
    public int GetCurrentLives()
    {
        return PlayerPrefs.GetInt("Lives");
    }

    // reduce una vida al jugador
    public void SpendLives()
    {   
        SFXManager.instance.PlaySFX(SFXManager.instance.deadSound, 0.5f);
        int newLives = GetCurrentLives() - 1;

        // guarda el nuevo valor
        PlayerPrefs.SetInt("Lives", newLives);

        // actualiza la interfaz
        if (uiControler != null)
            uiControler.UpdateLives();

        // si no hay vidas, pasa a derrota
        if (newLives <= 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("LoseScene_Jorge");
        }
    }

    // suma monedas y actualiza la ui
    public void AddCoin(int amount)
    {
        coins += amount;

        if (uiControler != null)
            uiControler.UpdateCoins(coins);
    }
}
}