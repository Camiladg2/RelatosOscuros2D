using UnityEngine;

public class LuciernagaCompanera : MonoBehaviour
{
    public Transform objetivo; // El Jugador
    public float radioOrbita = 1.5f;
    public float velocidadGiro = 2f;
    public float amplitudFlote = 0.3f;

    private float anguloInicial;

    private void Start()
    {
        // Cada luciérnaga empieza en un ángulo distinto para no ir pegadas
        anguloInicial = Random.Range(0f, 360f);
    }

    private void Update()
    {
        if (objetivo == null) return;

        float angulo = anguloInicial + Time.time * velocidadGiro;
        float offsetX = Mathf.Cos(angulo) * radioOrbita;
        float offsetY = Mathf.Sin(angulo * 1.5f) * amplitudFlote;

        Vector3 destino = objetivo.position + new Vector3(offsetX, offsetY + 0.5f, 0f);
        transform.position = Vector3.Lerp(transform.position, destino, Time.deltaTime * 3f);
    }
}