using UnityEngine;
namespace Gio.Minigame{
 public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip disparoClip;     // Asigna aquí: disparo_r
    public AudioClip choqueClip;      // Asigna aquí: choque_mn
    public AudioClip enemigoClip;     // Asigna aquí: enemigo
    public AudioClip coinClip;        // Asigna aquí: coin
    public AudioClip powerUpClip;     // Asigna aquí: powerUp
    public AudioClip ganarClip;       // Asigna aquí: win
    public AudioClip perderClip;      // Asigna aquí: perder
    public AudioClip gritoWeeeClip;   // Asigna aquí: azareel_james-weee-337908

    [Header("Música de Fondo")]
    public AudioClip musicaFondo1;    // Asigna aquí: musicafondo1
    public AudioClip musicaFondo2;    // Asigna aquí: musicafondo2
    public AudioClip musicaBoss;      // Asigna aquí: m_b

    private void Awake()
    {
        Instance = this;
    }

    public void PlayShootSound()
    {
        if (disparoClip != null) sfxSource.PlayOneShot(disparoClip, 0.5f);
    }

    public void PlayHitSound()
    {
        if (choqueClip != null) sfxSource.PlayOneShot(choqueClip, 0.7f);
    }

    public void PlayEnemySound()
    {
        if (enemigoClip != null) sfxSource.PlayOneShot(enemigoClip, 0.8f);
    }

    public void PlayCoinSound()
    {
        if (coinClip != null) sfxSource.PlayOneShot(coinClip, 0.8f);
    }

    public void PlayPowerUpSound()
    {
        if (powerUpClip != null) sfxSource.PlayOneShot(powerUpClip, 1f);
    }

    public void PlayWinSound()
    {
        if (ganarClip != null) sfxSource.PlayOneShot(ganarClip, 1f);
    }

    public void PlayLoseSound()
    {
        if (perderClip != null) sfxSource.PlayOneShot(perderClip, 1f);
    }

    public void PlayScreamSound()
    {
        if (gritoWeeeClip != null) sfxSource.PlayOneShot(gritoWeeeClip, 1f);
    }

    public void PlayBackgroundMusic(AudioClip clipMusica)
    {
        if (musicSource != null && clipMusica != null)
        {
            musicSource.clip = clipMusica;
            musicSource.loop = true;
            musicSource.volume = 0.4f; 
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}

}
