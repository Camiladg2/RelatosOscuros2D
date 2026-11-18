using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using TMPro;

public class CalderoFinal : MonoBehaviour
{
    [Header("1. Interacción y Avisos")]
    public KeyCode teclaInteractuar = KeyCode.E;
    public GameObject avisoInteractuar;
    public GameObject avisoFaltanEsferas;
    public TMP_Text textoFaltanEsferas;

    [Header("2. Cámara y Caldero")]
    public CinemachineCamera camaraCinematica;
    [Range(0.5f, 1f)]
    public float factorAcercamiento = 0.75f;
    public Transform puntoCentroCaldero;
    public ParticleSystem particulasCaldero;
    public SpriteRenderer spriteCaldero;
    public Color colorCalderoActivo = new Color(0.6f, 1f, 0.8f, 1f);

    [Header("3. Audio del Caldero")]
    public AudioClip sonidoMezcla; // Arrastra tu audio aquí si no usas el AudioSource directo

    [Header("4. Espiral de 9 Esferas")]
    public GameObject prefabEsferaVisual;
    public float radioInicialEspiral = 2.2f;
    public float tamanoEsferas = 0.6f;

    [Header("5. Destello y Transformación en Ente")]
    public GameObject visualNormalJugador;
    public GameObject visualEnteJugador;
    public Transform mascaraLuzJugador;
    public ParticleSystem particulasEnte;
    public CanvasGroup pantallaFlashBlanco;

    [Header("6. Fundido y Escena de Inicio")]
    public CanvasGroup pantallaFadeNegro;
    public string nombreEscenaInicio = "Inicio";

    private bool jugadorCerca = false;
    private bool cinematicaIniciada = false;
    private PlayerController2D playerController;
    private AudioSource audioSource;

    private void Start()
    {
        if (avisoInteractuar != null) avisoInteractuar.SetActive(false);
        if (avisoFaltanEsferas != null) avisoFaltanEsferas.SetActive(false);
        if (visualEnteJugador != null) visualEnteJugador.SetActive(false);
        if (puntoCentroCaldero == null) puntoCentroCaldero = transform;

        // Obtiene o añade automáticamente el AudioSource del caldero
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (jugadorCerca && !cinematicaIniciada && Input.GetKeyDown(teclaInteractuar))
        {
            // Recuerda cambiar 'true' por 'GameProgress.AllCollected' cuando termines tus pruebas
            if (GameProgress.AllCollected)
            {
                cinematicaIniciada = true;
                StartCoroutine(EjecutarCinematicaFinal());
            }
            else
            {
                if (avisoFaltanEsferas != null) avisoFaltanEsferas.SetActive(true);
                if (textoFaltanEsferas != null)
                    textoFaltanEsferas.text = $"Faltan esferas ({GameProgress.Total}/{GameProgress.TotalRequired})";
            }
        }
    }

    private IEnumerator EjecutarCinematicaFinal()
    {
        if (avisoInteractuar != null) avisoInteractuar.SetActive(false);
        if (avisoFaltanEsferas != null) avisoFaltanEsferas.SetActive(false);

        // --- REPRODUCIR SONIDO AL PRESIONAR 'E' ---
        if (sonidoMezcla != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoMezcla);
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // --- FASE 1: BLOQUEO Y ACERCAMIENTO SUAVE DE CÁMARA (1.2 s) ---
        if (playerController != null)
        {
            playerController.enabled = false;
            if (playerController.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        if (particulasCaldero != null) particulasCaldero.Play();
        if (spriteCaldero != null) spriteCaldero.color = colorCalderoActivo;

        float zoomInicial = camaraCinematica != null ? camaraCinematica.Lens.OrthographicSize : 3f;
        float zoomDestino = zoomInicial * factorAcercamiento;

        float t = 0f;
        while (t < 1.2f)
        {
            t += Time.deltaTime;
            if (camaraCinematica != null)
                camaraCinematica.Lens.OrthographicSize = Mathf.Lerp(zoomInicial, zoomDestino, t / 1.2f);
            yield return null;
        }

        // --- FASE 2: ESPIRAL DE LAS 9 ESFERAS HACIA EL CALDERO (2.8 s) ---
        List<Transform> esferas = new List<Transform>();
        if (prefabEsferaVisual != null)
        {
            for (int i = 0; i < 9; i++)
            {
                float angulo = (i * Mathf.PI * 2f) / 9f;
                Vector3 offset = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f) * radioInicialEspiral;
                GameObject esfera = Instantiate(prefabEsferaVisual, puntoCentroCaldero.position + offset, Quaternion.identity);

                esfera.transform.localScale = Vector3.one * tamanoEsferas;
                if (esfera.TryGetComponent(out SpriteRenderer sr))
                {
                    sr.sortingOrder = 60;
                    sr.color = new Color(0.4f, 1f, 0.8f, 1f);
                }

                esferas.Add(esfera.transform);
            }
        }

        t = 0f;
        float duracionEspiral = 2.8f;
        while (t < duracionEspiral)
        {
            t += Time.deltaTime;
            float progreso = t / duracionEspiral;
            float radioActual = Mathf.Lerp(radioInicialEspiral, 0f, progreso);
            float giroExtra = progreso * Mathf.PI * 6f;

            for (int i = 0; i < esferas.Count; i++)
            {
                if (esferas[i] == null) continue;
                float anguloBase = (i * Mathf.PI * 2f) / 9f + giroExtra;
                Vector3 posEspiral = new Vector3(Mathf.Cos(anguloBase), Mathf.Sin(anguloBase), 0f) * radioActual;
                esferas[i].position = puntoCentroCaldero.position + posEspiral;
                esferas[i].localScale = Vector3.Lerp(Vector3.one * tamanoEsferas, Vector3.zero, progreso);
            }
            yield return null;
        }

        foreach (Transform e in esferas)
            if (e != null) Destroy(e.gameObject);

        // --- FASE 3: ELEVACIÓN, DESTELLO BLANCO Y TRANSFORMACIÓN EN ENTE (1.8 s) ---
        Transform trJugador = playerController.transform;
        Vector3 posBaseJugador = trJugador.position;
        Vector3 posElevada = posBaseJugador + Vector3.up * 0.8f;
        Vector3 escalaLuzBase = mascaraLuzJugador != null ? mascaraLuzJugador.localScale : Vector3.one * 6f;

        t = 0f;
        while (t < 0.9f)
        {
            t += Time.deltaTime;
            float p = t / 0.9f;
            trJugador.position = Vector3.Lerp(posBaseJugador, posElevada, p);

            if (mascaraLuzJugador != null)
                mascaraLuzJugador.localScale = Vector3.Lerp(escalaLuzBase, escalaLuzBase * 4f, p);

            if (pantallaFlashBlanco != null)
                pantallaFlashBlanco.alpha = Mathf.Lerp(0f, 1f, p);

            yield return null;
        }

        if (visualNormalJugador != null) visualNormalJugador.SetActive(false);
        if (visualEnteJugador != null) visualEnteJugador.SetActive(true);
        if (particulasEnte != null) particulasEnte.Play();

        t = 0f;
        while (t < 0.9f)
        {
            t += Time.deltaTime;
            float p = t / 0.9f;
            if (mascaraLuzJugador != null)
                mascaraLuzJugador.localScale = Vector3.Lerp(escalaLuzBase * 4f, escalaLuzBase * 1.5f, p);

            if (pantallaFlashBlanco != null)
                pantallaFlashBlanco.alpha = Mathf.Lerp(1f, 0f, p);

            yield return null;
        }

        // --- FASE 4: CONTEMPLACIÓN DEL ENTE FLOTANDO Y FUNDIDO A INICIO (3.2 s) ---
        t = 0f;
        float duracionCierre = 3.2f;
        while (t < duracionCierre)
        {
            t += Time.deltaTime;
            float p = t / duracionCierre;

            float ondaFlote = Mathf.Sin(Time.time * 3f) * 0.18f;
            trJugador.position = posElevada + new Vector3(0f, ondaFlote, 0f);

            if (pantallaFadeNegro != null && p > 0.6f)
            {
                pantallaFadeNegro.alpha = Mathf.InverseLerp(0.6f, 1f, p);
            }

            yield return null;
        }

        // Limpiar progreso y volver a la escena de inicio
        GameProgress.ResetAll();
        PlayerPrefs.DeleteKey("NivelActual");
        PlayerPrefs.Save();
        SceneManager.LoadScene(nombreEscenaInicio);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cinematicaIniciada) return;

        PlayerController2D pc = other.GetComponentInParent<PlayerController2D>();
        if (pc != null)
        {
            jugadorCerca = true;
            playerController = pc;
            if (avisoInteractuar != null) avisoInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController2D>() != null)
        {
            jugadorCerca = false;
            if (avisoInteractuar != null) avisoInteractuar.SetActive(false);
            if (avisoFaltanEsferas != null) avisoFaltanEsferas.SetActive(false);
        }
    }
}