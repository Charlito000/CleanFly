using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Limpiable : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public GameObject panelUI;
    public Image imagenProgreso;

    [Header("Configuración")]
    public float distanciaActivacion = 3f;
    public float tiempoLimpiar = 2f;

    float progreso = 0f;
    bool limpiado = false;

    void Start()
    {
        panelUI.SetActive(false);
    }

    void Update()
    {
        if (limpiado) return;

        float dist = Vector3.Distance(transform.position, jugador.position);
        bool cerca = dist <= distanciaActivacion;

        // Mostrar u ocultar el panel
        panelUI.SetActive(cerca);

        // El panel siempre mira al jugador
        if (cerca)
        {
            panelUI.transform.LookAt(jugador);
            panelUI.transform.Rotate(0f, 180f, 0f);
        }

        // Progreso al mantener E
        if (cerca && Input.GetKey(KeyCode.E))
        {
            progreso += Time.deltaTime / tiempoLimpiar;
            progreso = Mathf.Clamp01(progreso);
        }
        else
        {
            progreso -= Time.deltaTime / tiempoLimpiar;
            progreso = Mathf.Clamp01(progreso);
        }

        imagenProgreso.fillAmount = progreso;

        if (progreso >= 1f)
            Limpiar();
    }

    void Limpiar()
    {
        limpiado = true;
        panelUI.SetActive(false);
        gameObject.SetActive(false);

        // Avisa al HUD que se limpió uno
        HUDManager.instancia.RegistrarLimpieza();

        // Sonido de limpieza
        if (AudioManager.instancia != null)
            AudioManager.instancia.PlayLimpiar();

        if (HUDManager.instancia != null)
            HUDManager.instancia.RegistrarLimpieza();
    }
}
