using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NivelManager : MonoBehaviour
{
    public static NivelManager instancia;

    [Header("Paneles")]
    public GameObject panelWin;
    public GameObject panelLose;

    [Header("Textos de tiempo")]
    public TextMeshProUGUI textoTiempoWin;
    public TextMeshProUGUI textoTiempoLose;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        panelWin.SetActive(false);
        panelLose.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Música del juego
        if (AudioManager.instancia != null)
            AudioManager.instancia.PlayMusica(AudioManager.instancia.musicaJuego);

        if (Narrador.instancia != null)
            Narrador.instancia.IniciarNarrador();
    }

  
    // ══ Win ══════════════════════════════════════════════════

    public void MostrarWin(string tiempo)
    {
        panelWin.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (textoTiempoWin != null)
            textoTiempoWin.text = " " + tiempo;

        // Sonido Win
        if (AudioManager.instancia != null)
        {
            AudioManager.instancia.DetenerMusica();
            AudioManager.instancia.PlayWin();
        }
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;

        if (siguienteEscena >= SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(siguienteEscena);
    }

    public void RepetirNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // ══ Lose ═════════════════════════════════════════════════

    public void MostrarLose(string tiempo)
    {
        panelLose.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (textoTiempoLose != null)
            textoTiempoLose.text = " " + tiempo;

        // Sonido Lose
        if (AudioManager.instancia != null)
        {
            AudioManager.instancia.DetenerMusica();
            AudioManager.instancia.PlayLose();
        }
    }
}