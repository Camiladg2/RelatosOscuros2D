using UnityEngine;

/// <summary>Pocion recogible: al tocarla, la protagonista recibe el poder y la pocion desaparece.</summary>
[RequireComponent(typeof(Collider2D))]
public class PotionPickup : MonoBehaviour
{
    public PowerType power = PowerType.DobleSalto;
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerPowerUps player = other.GetComponentInParent<PlayerPowerUps>();
        if (player == null) return;

        player.Activate(power);
        if (pickupEffect != null) Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        Destroy(gameObject);
    }
}
