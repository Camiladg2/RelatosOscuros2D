using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 1.2f;
    [Tooltip("Grados por segundo que gira mientras vuela (0 = no gira).")]
    public float spinSpeed = 360f;

    private Vector2 direction = Vector2.right;
    private int damage = 1;
    private WeaknessType weakness = WeaknessType.None;
    private bool used;

    public void Launch(Vector2 dir, int damageAmount, WeaknessType weaknessType, Sprite sprite)
    {
        direction = dir.normalized;
        damage = damageAmount;
        weakness = weaknessType;

        if (sprite != null)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.sprite = sprite;
        }

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (dir.x >= 0f ? 1f : -1f);
        transform.localScale = s;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (spinSpeed != 0f) transform.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (other.GetComponentInParent<PlayerController2D>() != null) return;

        IWeaknessReceiver weak = other.GetComponentInParent<IWeaknessReceiver>();
        IDamageable target = other.GetComponentInParent<IDamageable>();

        // Con un objeto de debilidad, el jefe decide que pasa (aturdirse, perder vida...).
        // Sin objeto (o contra enemigos sin debilidad, como las plantas), recibe dano normal.
        bool handledByWeakness = false;
        if (weak != null && weakness != WeaknessType.None)
        {
            weak.ApplyWeakness(weakness, transform.position);
            handledByWeakness = true;
        }
        if (!handledByWeakness && target != null)
            target.TakeDamage(damage, transform.position);

        // Se destruye al tocar un enemigo o cualquier cosa solida (suelo, paredes).
        if (weak != null || target != null || !other.isTrigger)
        {
            used = true;
            Destroy(gameObject);
        }
    }
}