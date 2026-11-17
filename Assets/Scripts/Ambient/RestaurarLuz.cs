using System.Collections;
using UnityEngine;

public class RestaurarLuz : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer oscuridadRenderer; // Arrastra aquí tu objeto OscuridadTotal
    public Transform mascaraLuzJugador;      // Opcional: Sprite Mask del jugador para asegurar su escala normal

    [Header("Configuración")]
    [Range(0, 255)]
    public int alphaNormal = 132;            // El Alpha original con el que empieza el nivel
    public float escalaNormalMascara = 6f;   // La escala inicial de tu Sprite Mask del jugador
    public float duracionTransicion = 1.5f;
    public bool retirarLuciernagas = true;   // Apaga las luciérnagas al volver la luz

    private bool yaRestaurado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!yaRestaurado && collision.CompareTag("Player"))
        {
            yaRestaurado = true;
            StartCoroutine(VolverALaNormalidad());
        }
    }

    private IEnumerator VolverALaNormalidad()
    {
        // 1. Retira las luciérnagas compañeras y guías activas si está marcada la opción
        if (retirarLuciernagas)
        {
            foreach (var comp in FindObjectsByType<LuciernagaCompanera>(FindObjectsSortMode.None))
                Destroy(comp.gameObject);

            foreach (var guia in FindObjectsByType<LuciernagaPuntoGuia>(FindObjectsSortMode.None))
                guia.gameObject.SetActive(false);
        }

        // 2. Devuelve suavemente el Alpha de OscuridadTotal a 132 (132 / 255)
        float alphaInicio = oscuridadRenderer != null ? oscuridadRenderer.color.a : 1f;
        float alphaDestino = alphaNormal / 255f;
        Vector3 escalaInicio = mascaraLuzJugador != null ? mascaraLuzJugador.localScale : Vector3.one;
        Vector3 escalaDestino = new Vector3(escalaNormalMascara, escalaNormalMascara, 1f);

        float tiempo = 0f;
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionTransicion;

            if (oscuridadRenderer != null)
            {
                Color c = oscuridadRenderer.color;
                c.a = Mathf.Lerp(alphaInicio, alphaDestino, t);
                oscuridadRenderer.color = c;
            }

            if (mascaraLuzJugador != null)
                mascaraLuzJugador.localScale = Vector3.Lerp(escalaInicio, escalaDestino, t);

            yield return null;
        }

        if (oscuridadRenderer != null)
        {
            Color c = oscuridadRenderer.color;
            c.a = alphaDestino;
            oscuridadRenderer.color = c;
        }
    }
}