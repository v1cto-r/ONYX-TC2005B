using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControler : MonoBehaviour
{
    // imagenes que representan las vidas (corazones)
    public Image[] livesImages;

    // sprite que se usa cuando se pierde una vida
    public Sprite spendLives;

    // texto donde se muestran las monedas
    public TextMeshProUGUI coinsText;

    void Start()
    {
        // al iniciar actualiza todas las vidas en pantalla
        UpdateAllLives();
    }

    public void UpdateLives()
    {
        // obtiene las vidas actuales
        int lives = GameControl.Instance.GetCurrentLives();

        // cambia solo el corazon correspondiente cuando pierde una vida
        if (lives >= 0 && lives < livesImages.Length)
        {
            livesImages[lives].sprite = spendLives;
        }
    }

    public void UpdateAllLives()
    {
        // actualiza todos los corazones al iniciar el juego
        int lives = GameControl.Instance.GetCurrentLives();

        for (int i = 0; i < livesImages.Length; i++)
        {
            if (i >= lives)
            {
                // los que ya no tiene se muestran como perdidos
                livesImages[i].sprite = spendLives;
            }
        }
    }

    public void UpdateCoins(int coins)
    {
        // muestra la cantidad de monedas en la interfaz
        coinsText.text = coins.ToString();
    }
}