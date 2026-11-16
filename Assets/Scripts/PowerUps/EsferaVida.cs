using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EsferaVida : MonoBehaviour
{
    public int curar = 1;
    public bool soloSiNecesitaVida = true;

    [Header("Levitar")]
    public float bobHeight = 0.12f;
    public float bobSpeed = 2f;

    [Header("Efectos (opcional)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;
    public UnityEvent alRecoger;

    private Vector3 startPosition;
    private float phase;
    private bool usada;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void Awake()
    {
        startPosition = transform.position;
        phase = startPosition.x;
    }

    private void Update()
    {
        if (bobHeight <= 0f) return;
        transform.position = startPosition + new Vector3(0f, Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryPickup(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryPickup(other);
    }

    private void TryPickup(Collider2D other)
    {
        if (usada) return;

        PlayerHealth vida = other.GetComponentInParent<PlayerHealth>();
        if (vida == null || vida.IsDead) return;

        if (soloSiNecesitaVida && vida.CurrentHealth >= vida.maxHealth) return;

        usada = true;
        vida.Heal(curar);

        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        alRecoger.Invoke();

        Destroy(gameObject);
    }
}
