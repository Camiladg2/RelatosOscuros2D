using UnityEngine;

public class LuciernagaPuntoGuia : MonoBehaviour
{
    [Header("Siguiente en el camino")]
    public LuciernagaPuntoGuia siguienteLuciernaga; // Arrastra aquí la siguiente luciérnaga guía

    [Header("Configuración Visual y Alcance")]
    public Color colorGuia = new Color(0.2f, 0.95f, 1f, 1f); // Azul cian brillante
    public float distanciaParaActivar = 1.5f; // Qué tan cerca debe llegar el jugador para "tocarla"
    public float velocidadParpadeo = 5f;

    private Transform jugador;
    private SpriteRenderer spriteRenderer;
    private Transform auraLuz;
    private Vector3 escalaBaseAura;
    private Vector3 posicionInicial;
    private bool completada = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = colorGuia;

        if (transform.childCount > 0)
        {
            auraLuz = transform.GetChild(0);
            escalaBaseAura = auraLuz.localScale;
        }

        posicionInicial = transform.position;
    }

    private void Start()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
            jugador = objJugador.transform;
    }

    private void Update()
    {
        if (completada) return;

        // 1. Flote suave en su sitio (como diciendo "¡alcánzame aquí!")
        float flotarY = Mathf.Sin(Time.time * 4f) * 0.25f;
        transform.position = posicionInicial + new Vector3(0f, flotarY, 0f);

        // 2. Parpadeo del punto y del halo de luz
        EfectoParpadeo();

        // 3. Esperar a que el jugador llegue hasta ella
        if (jugador != null)
        {
            float distancia = Vector2.Distance(jugador.position, transform.position);
            if (distancia <= distanciaParaActivar)
            {
                AlcanzadaPorJugador();
            }
        }
    }

    private void AlcanzadaPorJugador()
    {
        completada = true;

        // Enciende la siguiente luciérnaga guía si existe
        if (siguienteLuciernaga != null)
        {
            siguienteLuciernaga.gameObject.SetActive(true);
        }

        // Apaga esta luciérnaga actual
        gameObject.SetActive(false);
    }

    private void EfectoParpadeo()
    {
        float pulso = (Mathf.Sin(Time.time * velocidadParpadeo) + 1f) * 0.5f;

        if (spriteRenderer != null)
        {
            Color c = colorGuia;
            c.a = Mathf.Lerp(0.45f, 1f, pulso);
            spriteRenderer.color = c;
        }

        if (auraLuz != null)
        {
            float factorEscala = Mathf.Lerp(0.85f, 1.25f, pulso);
            auraLuz.localScale = escalaBaseAura * factorEscala;
        }
    }
}