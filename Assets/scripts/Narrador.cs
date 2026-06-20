using UnityEngine;
using System.Collections;

public class Narrador : MonoBehaviour
{
    public static Narrador instancia;

    [Header("Clips de inicio")]
    public AudioClip[] frasesInicio;

    [Header("Clips al recibir golpe")]
    public AudioClip[] frasesGolpe;

    [Header("Clips al limpiar")]
    public AudioClip[] frasesLimpiar;

    [Header("Clips con poca vida")]
    public AudioClip[] frasesPocaVida;

    [Header("Clips al ganar")]
    public AudioClip[] frasesGanar;

    [Header("Clips al perder")]
    public AudioClip[] frasesPerder;

    [Header("Configuración")]
    public float volumen = 1f;
    public float cooldownNarrador = 4f; // segundos mínimos entre frases

    AudioSource audioSource;
    bool puedeHablar = true;
    bool yaAvisoPocaVida = false;

    void Awake()
    {
        instancia = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = null;
        audioSource.Stop();        // <- fuerza que pare cualquier cosa
        audioSource.volume = volumen;
        puedeHablar = false; // <- bloqueado
    }

    void Start()
    {
        // No hace nada, espera que NivelManager llame IniciarNarrador
    }


    public void IniciarNarrador()
    {
        Debug.Log("IniciarNarrador llamado desde: " + new System.Diagnostics.StackTrace().ToString());
        puedeHablar = true; // <- ahora sí puede hablar
        StartCoroutine(FraseInicio());
    }

    IEnumerator FraseInicio()
    {
        yield return new WaitForSeconds(2f);
        Decir(frasesInicio);
    }

    // ══ Métodos públicos ═════════════════════════════════════

    public void OnGolpe() => Decir(frasesGolpe);
    public void OnLimpiar() => Decir(frasesLimpiar);
    public void OnGanar() => DecirForzado(frasesGanar);
    public void OnPerder() => DecirForzado(frasesPerder);

    public void OnPocaVida()
    {
        if (yaAvisoPocaVida) return;
        yaAvisoPocaVida = true;
        DecirForzado(frasesPocaVida);
    }

    // ══ Motor ════════════════════════════════════════════════

    // Respeta el cooldown para no interrumpir
    void Decir(AudioClip[] clips)
    {
        if (!puedeHablar) return;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        StartCoroutine(ReproducirConCooldown(clip));
    }

    // Interrumpe lo que haya y habla igual (para ganar/perder)
    void DecirForzado(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        StopAllCoroutines();
        audioSource.Stop();
        audioSource.PlayOneShot(clip, volumen);
    }

    IEnumerator ReproducirConCooldown(AudioClip clip)
    {
        puedeHablar = false;
        audioSource.PlayOneShot(clip, volumen);

        // Espera q termine el clip más el cooldown
        yield return new WaitForSeconds(clip.length + cooldownNarrador);
        puedeHablar = true;
    }
}