using UnityEngine;
using UnityEngine.UI;

public class IndicadorDano : MonoBehaviour
{
    public static IndicadorDano instancia;

    public float velocidadDesvanecimiento = 3f;

    Image panel;

    void Awake()
    {
        instancia = this;
        panel = GetComponent<Image>();

        // Fuerza el panel invisible al inicio
        Color c = panel.color;
        c.a = 0f;
        panel.color = c;
    }

    void Update()
    {
        // Se desvanece solo con el tiempo
        Color c = panel.color;
        c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * velocidadDesvanecimiento);
        panel.color = c;
    }

    public void MostrarDano()
    {
        Color c = panel.color;
        c.a = 0.6f; // aparece de golpe semitransparente
        panel.color = c;
    }
}
