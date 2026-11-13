using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaknessPickup : MonoBehaviour
{
    public WeaknessType type = WeaknessType.Linterna;
    [Tooltip("Cuantas veces se puede usar. Pon un numero grande (999) si solo quieres que lo limite el tiempo.")]
    public int charges = 999;
    [Tooltip("Segundos que lo lleva la protagonista antes de que desaparezca de su mano. 0 = sin limite de tiempo.")]
    public float duration = 20f;

    [Header("Levitar (sin animacion)")]
    [Tooltip("Cuanto sube y baja. 0 = quieto.")]
    public float bobHeight = 0.12f;
    [Tooltip("Que tan rapido sube y baja.")]
    public float bobSpeed = 2f;

    [Header("Efectos (opcional)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private Vector3 startPosition;
    private SpriteRenderer sprite;
    private float phase;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Awake()
    {
        startPosition = transform.position;
        sprite = GetComponentInChildren<SpriteRenderer>();
        phase = startPosition.x;
    }

    private void Update()
    {
        if (bobHeight <= 0f) return;
        transform.position = startPosition + new Vector3(0f, Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAttack attack = other.GetComponentInParent<PlayerAttack>();
        if (attack == null) return;

        Sprite icon = sprite != null ? sprite.sprite : null;
        attack.GiveItem(type, charges, icon, duration);
        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        Destroy(gameObject);
    }
}