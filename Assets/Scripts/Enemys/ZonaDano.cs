using System.Collections;
using UnityEngine;

public class ZonaDano : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;
    public float cooldown = 0.6f;
    [Tooltip("Si lo marcas, la zona mata de golpe (piso de muerte) en vez de quitar vida.")]
    public bool matar = false;

    [Header("Volver al punto seguro")]
    public bool returnToSafeSpot = true;
    public float returnDelay = 0.25f;
    public float liftOnReturn = 0.3f;

    private float readyTime;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null && p.GetComponent<SafeSpotTracker>() == null)
            p.AddComponent<SafeSpotTracker>();
    }

    private void OnTriggerEnter2D(Collider2D other) { Hit(other); }
    private void OnTriggerStay2D(Collider2D other) { Hit(other); }

    private void Hit(Collider2D other)
    {
        if (Time.time < readyTime) return;
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null || health.IsDead) return;

        readyTime = Time.time + cooldown;

        if (matar)
        {
            health.Kill();
            return;
        }

        health.TakeDamage(damage, transform.position, false);

        if (returnToSafeSpot && !health.IsDead)
            StartCoroutine(ReturnToSafe(health));
    }

    private IEnumerator ReturnToSafe(PlayerHealth health)
    {
        yield return new WaitForSeconds(returnDelay);

        if (health == null || health.IsDead) yield break;

        SafeSpotTracker tracker = health.GetComponent<SafeSpotTracker>();
        if (tracker == null || !tracker.hasSpot) yield break;

        Rigidbody2D rb = health.GetComponent<Rigidbody2D>();
        Vector2 target = tracker.safePosition + Vector2.up * liftOnReturn;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = target;
        }
        health.transform.position = new Vector3(target.x, target.y, health.transform.position.z);
        Physics2D.SyncTransforms();
    }
}

public class SafeSpotTracker : MonoBehaviour
{
    public Vector2 safePosition;
    public bool hasSpot;

    private PlayerController2D controller;
    private float groundedTime;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (controller == null) return;

        if (controller.IsGrounded)
        {
            groundedTime += Time.deltaTime;
            if (groundedTime > 0.35f)
            {
                safePosition = transform.position;
                hasSpot = true;
            }
        }
        else
        {
            groundedTime = 0f;
        }
    }
}