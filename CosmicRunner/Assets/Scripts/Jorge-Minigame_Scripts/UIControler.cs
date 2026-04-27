using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    // arreglo de imagenes de corazones
    public Image[] livesImages;

    // sprite que se usara cuando se pierde una vida (calavera)
    public Sprite spendLives;

    void Start()
    {
        // al iniciar se actualizan todas las vidas
        UpdateAllLives();
    }

    // se llama cada vez que se pierde una vida
    public void UpdateLives()
    {
        int lives = GameControl.Instance.GetCurrentLives();

        // cambia solo el corazon correspondiente a calavera
        if (lives >= 0 && lives < livesImages.Length)
        {
            livesImages[lives].sprite = spendLives;
        }
    }

    // actualiza todos los corazones al inicio
    public void UpdateAllLives()
    {
        int lives = GameControl.Instance.GetCurrentLives();

        for (int i = 0; i < livesImages.Length; i++)
        {
            if (i < lives)
            {
                // se queda como corazon normal
            }
            else
            {
                // se convierte en calavera
                livesImages[i].sprite = spendLives;
            }
        }
    }
}