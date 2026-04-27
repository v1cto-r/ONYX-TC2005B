using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    // instancia global para poder acceder desde otros scripts
    public static GameControl Instance;

    // numero de vidas iniciales
    public int initialLives = 5;

    // referencia al script de la interfaz (corazones)
    public UIControler uiControler;

    void Awake()
    {
        // se guarda esta instancia
        Instance = this;

        // se reinician las vidas al iniciar el juego
        PlayerPrefs.SetInt("Lives", initialLives);

        // si no esta asignado en inspector, lo busca automaticamente
        if (uiControler == null)
            uiControler = FindAnyObjectByType<UIControler>();

        // evita que este objeto se destruya al cambiar de escena
        DontDestroyOnLoad(this.gameObject);
    }

    // obtiene las vidas actuales guardadas
    public int GetCurrentLives()
    {
        return PlayerPrefs.GetInt("Lives");
    }

    // se llama cuando el jugador pierde una vida
    public void SpendLives()
    {
        int newLives = GetCurrentLives() - 1;

        // guarda el nuevo valor de vidas
        PlayerPrefs.SetInt("Lives", newLives);

        // actualiza la interfaz de corazones
        if (uiControler != null)
            uiControler.UpdateLives();

        // si ya no hay vidas se cambia a la escena final
       if (newLives <= 0)
{
    Debug.Log("game over");
    Time.timeScale = 0f; // pausa el juego
}
    }
}