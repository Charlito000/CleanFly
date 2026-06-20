using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Música de fondo")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;

    [Header("Efectos")]
    public AudioClip sonidoGolpe;
    public AudioClip sonidoLimpiar;
    public AudioClip sonidoVolar;
    public AudioClip sonidoWin;
    public AudioClip sonidoLose;

    [Header("Volúmenes")]
    [Range(0f, 1f)] public float volumenMusica = 0.5f;
    [Range(0f, 1f)] public float volumenEfectos = 0.8f;
    [Range(0f, 1f)] public float volumenVolar = 0.3f;

    AudioSource musicaSource;   // para música de fondo (loop)
    AudioSource efectosSource;  // para efectos cortos

    void Awake()
    {
        // Que no se destruya al cambiar de escena
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Crea dos AudioSource en el mismo objeto
        musicaSource = gameObject.AddComponent<AudioSource>();
        efectosSource = gameObject.AddComponent<AudioSource>();

        musicaSource.loop = true;
        musicaSource.volume = 0.5f;
        efectosSource.loop = false;

        musicaSource.volume = volumenMusica;
        efectosSource.volume = volumenEfectos;
    }

    // ══ Música ═══════════════════════════════════════════════

    public void PlayMusica(AudioClip clip)
    {
        if (musicaSource.clip == clip) return;
        musicaSource.clip = clip;
        musicaSource.Play();
    }

    public void PausarMusica()
    {
        musicaSource.Pause();
    }

    public void ReanudarMusica()
    {
        musicaSource.UnPause();
    }

    public void DetenerMusica()
    {
        musicaSource.Stop();
    }

    // ══ Efectos ══════════════════════════════════════════════

    public void PlayEfecto(AudioClip clip, float volumen = 1f)
    {
        if (clip == null) return;
        efectosSource.PlayOneShot(clip, volumen);
    }

    // ══ Métodos específicos ═══════════════════════════════════

    public void PlayGolpe() => PlayEfecto(sonidoGolpe);
    public void PlayLimpiar() => PlayEfecto(sonidoLimpiar);
    public void PlayWin() => PlayEfecto(sonidoWin);
    public void PlayLose() => PlayEfecto(sonidoLose);

    // Volar es un loop, necesita su propio AudioSource
    AudioSource volarSource;
    public void IniciarVolar()
    {
        if (volarSource == null)
        {
            volarSource = gameObject.AddComponent<AudioSource>();
            volarSource.clip = sonidoVolar;
            volarSource.loop = true;
            volarSource.volume = 0.4f;
        }
        if (!volarSource.isPlaying)
            volarSource.Play();
        volarSource.volume = volumenVolar;
    }

    public void DetenerVolar()
    {
        if (volarSource != null && volarSource.isPlaying)
            volarSource.Stop();
    }
}