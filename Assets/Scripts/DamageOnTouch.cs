using UnityEngine;

/// <summary>
/// Hace dano a la protagonista al tocarla. Sirve para pinchos, acido y para probar el dano.
/// Ponlo en un objeto con Collider2D (activa "Is Trigger" si quieres que sea una zona).
/// </summary>
public class DamageOnTouch : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        Hit(other);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Hit(collision.collider);
    }

    private void Hit(Collider2D other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health != null) health.TakeDamage(damage, transform.position);
    }
}
