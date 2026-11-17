using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaCombateOscuro : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer oscuridadRenderer; // Arrastra aquí tu objeto OscuridadTotal
    public Transform mascaraLuzJugador;      // Arrastra aquí la Sprite Mask hija del Jugador
    public GameObject prefabLuciernaga;        // Arrastra tu Prefab Luciernaga

    [Header("Ajustes de Visión en Pelea")]
    [Range(0, 255)]
    public int alphaEnPelea = 160;        // Más bajo = más claro para ver la pelea (ej. 160 de 255)
    public float escalaLuzCombate = 11f;  // Tamaño del círculo de luz durante la pelea
    public int luciernagasExtra = 4;      // Luciérnagas de apoyo en la arena

    private Vector3 escalaNormal;
    private List<GameObject> luciernagasTemporales = new List<GameObject>();
    private Coroutine corrutinaTransicion;

    private void Start()
    {
        if (mascaraLuzJugador != null)
            escalaNormal = mascaraLuzJugador.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float alphaObjetivo = alphaEnPelea / 255f;

            if (corrutinaTransicion != null) StopCoroutine(corrutinaTransicion);
            corrutinaTransicion = StartCoroutine(TransicionArena(new Vector3(escalaLuzCombate, escalaLuzCombate, 1f), alphaObjetivo));

            if (prefabLuciernaga != null && luciernagasTemporales.Count == 0)
            {
                for (int i = 0; i < luciernagasExtra; i++)
                {
                    GameObject extra = Instantiate(prefabLuciernaga, collision.transform.position, Quaternion.identity);
                    var comp = extra.AddComponent<LuciernagaCompanera>();
                    comp.objetivo = collision.transform;
                    comp.radioOrbita = Random.Range(2.0f, 3.5f);
                    comp.velocidadGiro = Random.Range(2.0f, 3.2f);

                    if (extra.TryGetComponent(out TrailRenderer tr))
                        tr.enabled = false;

                    luciernagasTemporales.Add(extra);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Al salir de la arena vuelve a oscuridad cerrada (Alpha 255 = 1f) hasta llegar al punto final
            if (corrutinaTransicion != null) StopCoroutine(corrutinaTransicion);
            corrutinaTransicion = StartCoroutine(TransicionArena(escalaNormal, 1f));

            foreach (GameObject luc in luciernagasTemporales)
            {
                if (luc != null) Destroy(luc);
            }
            luciernagasTemporales.Clear();
        }
    }

    private IEnumerator TransicionArena(Vector3 escalaDestino, float alphaDestino)
    {
        Vector3 escalaInicio = mascaraLuzJugador != null ? mascaraLuzJugador.localScale : Vector3.one;
        float alphaInicio = oscuridadRenderer != null ? oscuridadRenderer.color.a : 1f;

        float tiempo = 0f;
        float duracion = 0.8f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            if (mascaraLuzJugador != null)
                mascaraLuzJugador.localScale = Vector3.Lerp(escalaInicio, escalaDestino, t);

            if (oscuridadRenderer != null)
            {
                Color c = oscuridadRenderer.color;
                c.a = Mathf.Lerp(alphaInicio, alphaDestino, t);
                oscuridadRenderer.color = c;
            }

            yield return null;
        }

        if (mascaraLuzJugador != null)
            mascaraLuzJugador.localScale = escalaDestino;

        if (oscuridadRenderer != null)
        {
            Color c = oscuridadRenderer.color;
            c.a = alphaDestino;
            oscuridadRenderer.color = c;
        }
    }
}