using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Piso falso ("Piso Aleja"): cuando la protagonista lo pisa tiembla, se desintegra entre la neblina,
/// ella cae y es teletransportada a otra dimension (otro punto de la escena o otra escena).
/// Ponlo en una plataforma con Collider2D (sin Is Trigger).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FalsePortalFloor : MonoBehaviour
{
    [Header("A donde lleva (elige una)")]
    [Tooltip("Un objeto vacio colocado en la otra dimension, dentro de esta misma escena.")]
    public Transform destination;
    [Tooltip("O el nombre de otra escena. Debe estar en File > Build Settings.")]
    public string sceneToLoad = "";

    [Header("Tiempos (segundos)")]
    public float triggerDelay = 0.4f;
    public float shakeTime = 0.9f;
    public float shakeAmount = 0.05f;
    public float dissolveTime = 0.8f;
    [Tooltip("Cuanto tiempo cae antes de teletransportarla.")]
    public float teleportDelay = 0.7f;
    [Tooltip("Si es mayor que 0, el piso reaparece despues de estos segundos. 0 = desaparece para siempre.")]
    public float respawnTime = 0f;

    [Header("Efectos (opcional)")]
    [Tooltip("Tu efecto de neblina 'Piso Aleja'. Debe empezar desactivado.")]
    public GameObject dissolveEffect;
    [Tooltip("Un CanvasGroup con una imagen negra a pantalla completa, para oscurecer al teletransportar.")]
    public CanvasGroup screenFade;
    public float fadeTime = 0.4f;
    public AudioClip sound;
    public UnityEvent onDissolveStart;
    public UnityEvent onTeleported;

    private Collider2D myCollider;
    private SpriteRenderer[] sprites;
    private bool triggered;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        if (dissolveEffect != null) dissolveEffect.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        PlayerController2D player = collision.collider.GetComponentInParent<PlayerController2D>();
        if (player == null) return;

        // Solo cuenta si pisa por arriba (no al golpear de lado o desde abajo).
        bool fromAbove = collision.collider.bounds.min.y >= myCollider.bounds.max.y - 0.3f;
        if (!fromAbove) return;

        triggered = true;
        StartCoroutine(Sequence(player));
    }

    private IEnumerator Sequence(PlayerController2D player)
    {
        yield return new WaitForSeconds(triggerDelay);

        // 1. Tiembla
        Vector3 basePosition = transform.position;
        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            transform.position = basePosition + new Vector3(Mathf.Sin(Time.time * 70f) * shakeAmount, 0f, 0f);
            yield return null;
        }
        transform.position = basePosition;

        // 2. Se desintegra: sin collider, se desvanece y sale la neblina
        if (onDissolveStart != null) onDissolveStart.Invoke();
        if (dissolveEffect != null) dissolveEffect.SetActive(true);
        if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
        myCollider.enabled = false;
        float d = 0f;
        while (d < dissolveTime)
        {
            d += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(d / dissolveTime));
            yield return null;
        }
        SetAlpha(0f);

        // 3. Cae un momento y se teletransporta
        yield return new WaitForSeconds(teleportDelay);
        player.SetControl(false);
        yield return Fade(1f);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            yield break;
        }

        if (destination != null)
        {
            player.Stop();
            player.transform.position = destination.position;
        }
        if (onTeleported != null) onTeleported.Invoke();
        yield return Fade(0f);
        player.SetControl(true);

        // 4. Opcional: el piso reaparece
        if (respawnTime > 0f)
        {
            yield return new WaitForSeconds(respawnTime);
            SetAlpha(1f);
            myCollider.enabled = true;
            if (dissolveEffect != null) dissolveEffect.SetActive(false);
            triggered = false;
        }
    }

    private IEnumerator Fade(float target)
    {
        if (screenFade == null) yield break;
        float start = screenFade.alpha;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            screenFade.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(t / fadeTime));
            yield return null;
        }
        screenFade.alpha = target;
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer s in sprites)
        {
            if (s == null) continue;
            Color c = s.color;
            c.a = alpha;
            s.color = c;
        }
    }
}
