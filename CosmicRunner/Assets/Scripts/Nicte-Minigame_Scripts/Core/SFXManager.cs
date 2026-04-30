using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public AudioClip bullet;
    public AudioClip damage;
    public AudioClip win;
    public AudioClip defeat;
    public AudioClip game;
    public AudioClip powerUp;
    public AudioClip coin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BulletSound()
    {
        AudioSource.PlayClipAtPoint(bullet, Camera.main.transform.position, 0.5f);
    }

    public void DamageSound()
    {
        AudioSource.PlayClipAtPoint(damage, Camera.main.transform.position, 0.5f);
    }

    public void WinSound()
    {
        AudioSource.PlayClipAtPoint(win, Camera.main.transform.position, 0.5f);
    }

    public void DefeatSound()
    {
        AudioSource.PlayClipAtPoint(defeat, Camera.main.transform.position, 0.5f);
    }

    public void GameSound()
    {
        AudioSource.PlayClipAtPoint(game, Camera.main.transform.position, 0.5f);
    }

    public void PowerUpSound()
    {
        AudioSource.PlayClipAtPoint(powerUp, Camera.main.transform.position, 0.5f);
    }

    public void CoinSound()
    {
        AudioSource.PlayClipAtPoint(coin, Camera.main.transform.position, 0.5f);
    }
}
