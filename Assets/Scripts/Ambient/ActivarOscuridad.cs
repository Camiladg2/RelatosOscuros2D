using System.Collections;
using UnityEngine;

public class ActivarOscuridad : MonoBehaviour
{
    [Header("Referencias Oscuridad")]
    public SpriteRenderer oscuridadRenderer;
    public float duracionTransicion = 1.5f;

    [Header("Luciérnagas Compañeras (Siguen al jugador)")]
    public GameObject prefabLuciernagaCompanera; // Arrastra tu Prefab Luciernaga
    public Transform jugador;
    public int cantidadCompaneras = 3;

    [Header("Primera Luciérnaga Guía")]
    public LuciernagaPuntoGuia primeraLuciernagaGuia; // Arrastra aquí la Guía #1 de la escena

    private bool yaActivado = false;
    private float alphaInicial;

    private void Start()
    {
        if (oscuridadRenderer != null)
            alphaInicial = oscuridadRenderer.color.a;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!yaActivado && collision.CompareTag("Player"))
        {
            yaActivado = true;
            StartCoroutine(OscurecerSuavemente());
            IniciarSistemaLuciernagas();
        }
    }

    private void IniciarSistemaLuciernagas()
    {
        // 1. Crea las 3 luciérnagas doradas alrededor del jugador
        if (prefabLuciernagaCompanera != null && jugador != null)
        {
            for (int i = 0; i < cantidadCompaneras; i++)
            {
                GameObject comp = Instantiate(prefabLuciernagaCompanera, jugador.position, Quaternion.identity);
                var scriptComp = comp.AddComponent<LuciernagaCompanera>();
                scriptComp.objetivo = jugador;
                scriptComp.radioOrbita = Random.Range(1.2f, 2.2f);
                scriptComp.velocidadGiro = Random.Range(1.5f, 2.5f);

                if (comp.TryGetComponent(out TrailRenderer tr))
                    tr.enabled = false;
            }
        }

        // 2. Enciende la primera luciérnaga guía del camino
        if (primeraLuciernagaGuia != null)
        {
            primeraLuciernagaGuia.gameObject.SetActive(true);
        }
    }

    private IEnumerator OscurecerSuavemente()
    {
        float tiempo = 0f;
        Color colorActual = oscuridadRenderer.color;

        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            colorActual.a = Mathf.Lerp(alphaInicial, 1f, tiempo / duracionTransicion);
            oscuridadRenderer.color = colorActual;
            yield return null;
        }

        colorActual.a = 1f;
        oscuridadRenderer.color = colorActual;
    }
}