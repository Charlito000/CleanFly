using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instancia;

    [Header("Vida")]
    public Image rellenoVida;
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;

    [Header("Temporizador")]
    public TextMeshProUGUI textoTiempo;
    bool temporizadorActivo = true;
    float tiempoTranscurrido = 0f;

    [Header("Limpieza")]
    public Image rellenoLimpieza;

    int totalMugres = 0;
    int mugresLimpiados = 0;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        // Cuenta automáticamente todos los Limpiable en la escena
        totalMugres = FindObjectsOfType<Limpiable>().Length;
        Debug.Log("Total mugres en escena: " + totalMugres);

        ActualizarVida();
        ActualizarLimpieza();
    }

    void Update()
    {
        if (!temporizadorActivo) return;

        tiempoTranscurrido += Time.deltaTime;

        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60f);
        textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // ── Vida ─────────────────────────────────────────────
    public void RecibirDano(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
        ActualizarVida();

        // Muestra el flash rojo
        if (IndicadorDano.instancia != null)
            IndicadorDano.instancia.MostrarDano();

        if (vidaActual <= 0f)
            MorirJugador();

        // Sonido de golpe
        if (AudioManager.instancia != null)
            AudioManager.instancia.PlayGolpe();

        if (vidaActual <= 0f)
            MorirJugador();
    }

    public void RecuperarVida(float cantidad)
    {
        vidaActual += cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
        ActualizarVida();
    }

    void ActualizarVida()
    {
        rellenoVida.fillAmount = vidaActual / vidaMaxima;
    }



    void MorirJugador()
    {
        if (NivelManager.instancia != null)
            NivelManager.instancia.MostrarLose(textoTiempo.text);
    }

    // ── Limpieza ─────────────────────────────────────────
    public void RegistrarLimpieza()
    {
        mugresLimpiados++;
        Debug.Log("Limpiados: " + mugresLimpiados + "/" + totalMugres);
        ActualizarLimpieza();

        if (mugresLimpiados >= totalMugres)
            LimpiezaCompleta();
    }

    void ActualizarLimpieza()
    {
        if (totalMugres <= 0) return;
        rellenoLimpieza.fillAmount = (float)mugresLimpiados / totalMugres;
    }

    void LimpiezaCompleta()
    {
        temporizadorActivo = false;
        if (NivelManager.instancia != null)
            NivelManager.instancia.MostrarWin(textoTiempo.text);
    }
}


