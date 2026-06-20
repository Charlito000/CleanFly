using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaManager : MonoBehaviour
{
    public GameObject panelPausa;
    bool pausado = false;

    void Start()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Continuar();
            else Pausar();
        }
    }

    void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;  // congela el juego
        panelPausa.SetActive(true);

        // Muestra el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pausa la música
    if (AudioManager.instancia != null)
        AudioManager.instancia.PausarMusica();
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;  // reanuda el juego
        panelPausa.SetActive(false);

        // Oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reanuda la música
        if (AudioManager.instancia != null)
            AudioManager.instancia.ReanudarMusica();
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f; // importante resetear antes de cambiar escena
        SceneManager.LoadScene(0); // carga la escena del menú principal
    }
}
