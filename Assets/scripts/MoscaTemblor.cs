using UnityEngine;

public class MoscaTemblor : MonoBehaviour
{
    public float amplitud = 0.08f;
    public float velocidad = 4f;

    Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.localPosition = posicionInicial + new Vector3(0f, offsetY, 0f);
    }
}
