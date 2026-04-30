using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Clips")]
    public AudioClip backgroundMusic;
    public AudioClip coinSound;
    public AudioClip checkpointSound;
    public AudioClip deadSound;

    // establece el singleton y evita duplicados entre escenas
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // mantiene este objeto entre cambios de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // reproducir música de fondo
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            musicSource.Play();
        }
    }

    // reproduce un efecto de sonido con volumen y tono configurables
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }

    // inicia la musica de fondo en bucle
    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.35f;
            musicSource.Play();
        }
    }
}