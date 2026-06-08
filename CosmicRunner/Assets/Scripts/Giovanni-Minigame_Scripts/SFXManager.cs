using UnityEngine;
namespace Gio.Minigame{
 public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip disparoClip;    
    public AudioClip choqueClip;      
    public AudioClip enemigoClip;    
    public AudioClip coinClip;        
    public AudioClip powerUpClip;     
    public AudioClip ganarClip;       
    public AudioClip perderClip;      
    public AudioClip gritoWeeeClip;   

    [Header("Música de Fondo")]
    public AudioClip musicaFondo1;   
    public AudioClip musicaFondo2;    
    public AudioClip musicaBoss;      // m_b

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
