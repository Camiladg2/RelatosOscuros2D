using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// PORTAL-PUERTA: la niña camina hacia el portal, la pantalla se oscurece,
/// aparece en otro lugar (o en otra escena) y la pantalla se aclara.
/// Va en el objeto del portal, que debe tener un Collider2D con "Is Trigger" activado.
/// </summary>
public class PortalPuerta : MonoBehaviour
{
    [Header("A dónde lleva (usa UNA de las dos)")]
    [Tooltip("Opción 1: un objeto vacío en esta misma escena.")]
    public Transform destination;
    [Tooltip("Opción 2: nombre EXACTO de otra escena (debe estar en Build Profiles > Scene List).")]
    public string sceneToLoad = "";

    [Header("Tiempos (segundos)")]
    public float enterDelay = 0.15f;
    public float fadeOutTime = 0.5f;
    public float blackTime = 0.25f;
    public float fadeInTime = 0.5f;
    public float cooldown = 1.0f;

    [Header("Opcionales")]
    [Tooltip("CanvasGroup de un panel negro que cubre toda la pantalla (Alpha 0).")]
    public CanvasGroup screenFade;
    [Tooltip("Se activa mientras la niña entra (partículas, brillo...).")]
    public GameObject enterEffect;
    public UnityEvent onEnter;
    public UnityEvent onTeleported;

    [Header("Brillo del portal (opcional)")]
    public SpriteRenderer glow;
    public float glowSpeed = 2f;
    [Range(0f, 1f)] public float glowMinAlpha = 0.75f;

    bool busy;
    float readyTime;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void Start()
    {
        if (screenFade != null)
        {
            screenFade.alpha = 0f;
            screenFade.blocksRaycasts = false;
        }
        if (enterEffect != null) enterEffect.SetActive(false);
    }

    void Update()
    {
        if (glow == null) return;
        float t = (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f;
        Color c = glow.color;
        c.a = Mathf.Lerp(glowMinAlpha, 1f, t);
        glow.color = c;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryEnter(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryEnter(other);
    }

    void TryEnter(Collider2D other)
    {
        if (busy || Time.time < readyTime) return;

        // DIAGNOSTICO: se escribe una sola vez por intento (solo si no esta ocupado)
        if (!other.CompareTag("Player"))
        {
            Debug.Log("[Portal] Toca: " + other.name + " | tag: " + other.tag + " (no es Player, se ignora)", this);
            return;
        }

        Transform player = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        Debug.Log("[Portal] Detecta a la niña: " + player.name, this);
        StartCoroutine(Travel(player));
    }

    IEnumerator Travel(Transform player)
    {
        busy = true;
        Debug.Log("[Portal] Empieza el viaje. Escena destino: '" + sceneToLoad + "'", this);
        onEnter.Invoke();
        if (enterEffect != null) enterEffect.SetActive(true);

        var pc = player.GetComponent<PlayerController2D>();
        var rb = player.GetComponent<Rigidbody2D>();
        if (pc != null) pc.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSecondsRealtime(enterDelay);
        yield return Fade(0f, 1f, fadeOutTime);
        yield return new WaitForSecondsRealtime(blackTime);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
            {
                Debug.LogError("[Portal] La escena '" + sceneToLoad + "' NO esta en Build Profiles > Scene List, o el nombre esta mal escrito.", this);
                // Deja a la niña libre otra vez para que no se quede congelada
                if (screenFade != null) screenFade.alpha = 0f;
                if (pc != null) pc.enabled = true;
                readyTime = Time.time + cooldown;
                busy = false;
                yield break;
            }

            Debug.Log("[Portal] Cargando escena: " + sceneToLoad, this);
            SceneManager.LoadScene(sceneToLoad);
            yield break;
        }

        if (destination != null)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = destination.position;
            }
            player.position = destination.position;
            Physics2D.SyncTransforms();
        }
        else
        {
            Debug.LogWarning("[Portal] No hay Destination ni Scene To Load.", this);
        }

        onTeleported.Invoke();
        if (enterEffect != null) enterEffect.SetActive(false);

        yield return Fade(1f, 0f, fadeInTime);

        if (pc != null) pc.enabled = true;
        readyTime = Time.time + cooldown;
        busy = false;
    }

    IEnumerator Fade(float from, float to, float time)
    {
        if (screenFade == null)
        {
            yield return new WaitForSecondsRealtime(time);
            yield break;
        }

        screenFade.blocksRaycasts = true;
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            screenFade.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / time));
            yield return null;
        }
        screenFade.alpha = to;
        screenFade.blocksRaycasts = to > 0.5f;
    }
}