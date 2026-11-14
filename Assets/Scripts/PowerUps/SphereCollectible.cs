using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Esfera de pocion recogible. Al tocarla la protagonista, suma al contador y se guarda.
/// Si ya la recogiste en una partida anterior, no vuelve a aparecer.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SphereCollectible : MonoBehaviour
{
    [Tooltip("Mundo al que pertenece: 1, 2 o 3.")]
    public int world = 1;
    [Tooltip("Dejalo vacio: se crea solo con el nombre de la escena y la posicion de la esfera.")]
    public string id = "";
    [Tooltip("Si esta marcado, la esfera esta oculta hasta que se llame a Reveal() (por ejemplo, con una palanca).")]
    public bool startHidden = false;

    [Header("Movimiento suave")]
    public float bobHeight = 0.15f;
    public float bobSpeed = 2f;

    [Header("Efectos (opcional)")]
    public GameObject collectEffect;
    public AudioClip collectSound;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool collected;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPosition = transform.position;
        if (string.IsNullOrEmpty(id)) id = BuildId();
    }

    private void Start()
    {
        if (GameProgress.IsCollected(id))
        {
            gameObject.SetActive(false);
            return;
        }
        if (startHidden) SetVisible(false);
    }

    private void Update()
    {
        if (bobHeight <= 0f) return;
        transform.position = startPosition + new Vector3(0f, Mathf.Sin(Time.time * bobSpeed) * bobHeight, 0f);
    }

    private string BuildId()
    {
        return SceneManager.GetActiveScene().name + "_" +
               Mathf.RoundToInt(transform.position.x * 10f) + "_" +
               Mathf.RoundToInt(transform.position.y * 10f);
    }

    /// <summary>Muestra una esfera oculta. Conectalo a un evento (palanca, puerta...).</summary>
    public void Reveal()
    {
        startHidden = false;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (col != null) col.enabled = visible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || startHidden) return;
        if (other.GetComponentInParent<PlayerController2D>() == null) return;

        collected = true;
        GameProgress.Collect(id, world);
        if (collectEffect != null) Instantiate(collectEffect, transform.position, Quaternion.identity);
        if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, transform.position);
        gameObject.SetActive(false);
    }
}
