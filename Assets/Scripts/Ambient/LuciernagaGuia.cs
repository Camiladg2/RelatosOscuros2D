using UnityEngine;

public class LuciernagaGuia : MonoBehaviour
{
    [Header("Movimiento y Ruta")]
    public Transform[] puntosCamino;
    public Transform jugador;
    public float velocidad = 3.5f;
    public float distanciaParaAvanzar = 3.5f;

    [Header("Apariencia Guía")]
    public Color colorGuia = new Color(0.2f, 0.95f, 1f, 1f); // Azul cian brillante (puedes cambiarlo en el Inspector)
    public float velocidadParpadeo = 5f;

    private int indiceActual = 0;
    private SpriteRenderer spriteRenderer;
    private Transform auraLuz;
    private Vector3 escalaBaseAura;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorGuia;
        }

        // Busca el objeto hijo AuraLuz (donde pusiste la Sprite Mask) para hacerla palpitar
        if (transform.childCount > 0)
        {
            auraLuz = transform.GetChild(0);
            escalaBaseAura = auraLuz.localScale;
        }
    }

    private void Update()
    {
        EfectoParpadeo();

        if (puntosCamino.Length == 0 || indiceActual >= puntosCamino.Length) return;

        Transform destino = puntosCamino[indiceActual];
        bool yaLlegoAlPunto = Vector2.Distance(transform.position, destino.position) < 0.35f;

        // Si ya llegó al punto y espera al jugador, rebota más marcado para llamar la atención
        float frecuenciaFlote = yaLlegoAlPunto ? 7f : 4f;
        float amplitudFlote = yaLlegoAlPunto ? 0.45f : 0.2f;
        float flotarY = Mathf.Sin(Time.time * frecuenciaFlote) * amplitudFlote;

        Vector3 posicionObjetivo = new Vector3(destino.position.x, destino.position.y + flotarY, 0f);
        transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidad * Time.deltaTime);

        if (yaLlegoAlPunto && jugador != null)
        {
            if (Vector2.Distance(jugador.position, transform.position) <= distanciaParaAvanzar)
            {
                indiceActual++;
            }
        }
    }

    private void EfectoParpadeo()
    {
        // Onda suave entre 0 y 1 para el parpadeo
        float pulso = (Mathf.Sin(Time.time * velocidadParpadeo) + 1f) * 0.5f;

        // Parpadea ligeramente el brillo/opacidad del punto central
        if (spriteRenderer != null)
        {
            Color c = colorGuia;
            c.a = Mathf.Lerp(0.45f, 1f, pulso);
            spriteRenderer.color = c;
        }

        // Hace crecer y encoger suavemente el círculo de luz (Sprite Mask)
        if (auraLuz != null)
        {
            float factorEscala = Mathf.Lerp(0.85f, 1.25f, pulso);
            auraLuz.localScale = escalaBaseAura * factorEscala;
        }
    }
}