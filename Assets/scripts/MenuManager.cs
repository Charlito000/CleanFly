using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelIntro;

    [Header("Referencias Intro")]
    public Image imagenFondo;
    public TextMeshProUGUI textoSubtitulo;
    public float tiempoFade = 0.5f;
    public float velocidadTexto = 0.05f; // segundos entre letra y letra
    public AudioClip sonidoTecleo;              // sonido opcional por letra

    [Header("Diapositivas")]
    public DiapositvaIntro[] diapositivas;

    [System.Serializable]
    public class DiapositvaIntro
    {
        public Sprite imagen;
        [TextArea]
        public string subtitulo;
        public float duracionDespuesDeTexto = 2f; // espera tras terminar el texto
        public AudioClip audio;
    }

    bool textoCompleto = false;
    bool saltarDialogo = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelMenu.SetActive(true);
        panelIntro.SetActive(false);

        if (AudioManager.instancia != null)
            AudioManager.instancia.PlayMusica(AudioManager.instancia.musicaMenu);
    }

    void Update()
    {
        // Click o espacio para completar el texto de golpe
        if (panelIntro.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!textoCompleto)
                    saltarDialogo = true;  // completa el texto instantáneo
            }
        }
    }

    // ══ Menú ═════════════════════════════════════════════════

    public void Jugar()
    {
        panelMenu.SetActive(false);
        panelIntro.SetActive(true);
        StartCoroutine(ReproducirIntro());
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir");
    }

    // ══ Intro ════════════════════════════════════════════════

    IEnumerator ReproducirIntro()
    {
        foreach (var diap in diapositivas)
        {
            // Fade in de la imagen
            yield return StartCoroutine(FadeImagen(0f, 1f, diap.imagen));

            // Audio opcional
            if (diap.audio != null && AudioManager.instancia != null)
                AudioManager.instancia.PlayEfecto(diap.audio);

            // Efecto typewriter
            yield return StartCoroutine(EscribirTexto(diap.subtitulo));

            // Espera después de que termina el texto
            yield return new WaitForSeconds(diap.duracionDespuesDeTexto);

            // Fade out
            yield return StartCoroutine(FadeImagen(1f, 0f, diap.imagen));
            textoSubtitulo.text = "";
        }

        IrAlJuego();
    }

    IEnumerator EscribirTexto(string texto)
    {
        textoCompleto = false;
        saltarDialogo = false;
        textoSubtitulo.text = "";

        foreach (char letra in texto)
        {
            if (saltarDialogo)
            {
                // Muestra todo el texto de golpe
                textoSubtitulo.text = texto;
                break;
            }

            textoSubtitulo.text += letra;

            // Sonido de tecleo opcional
            if (sonidoTecleo != null && AudioManager.instancia != null)
                AudioManager.instancia.PlayEfecto(sonidoTecleo, 0.3f);

            yield return new WaitForSeconds(velocidadTexto);
        }

        textoCompleto = true;
    }

    IEnumerator FadeImagen(float desde, float hasta, Sprite sprite)
    {
        imagenFondo.sprite = sprite;
        float t = 0f;
        while (t < tiempoFade)
        {
            t += Time.deltaTime;
            Color c = imagenFondo.color;
            c.a = Mathf.Lerp(desde, hasta, t / tiempoFade);
            imagenFondo.color = c;
            yield return null;
        }
        Color cf = imagenFondo.color;
        cf.a = hasta;
        imagenFondo.color = cf;
    }

    public void SaltarIntro()
    {
        StopAllCoroutines();
        textoSubtitulo.text = "";
        IrAlJuego();
    }

    void IrAlJuego()
    {
        SceneManager.LoadScene(1);
    }
}