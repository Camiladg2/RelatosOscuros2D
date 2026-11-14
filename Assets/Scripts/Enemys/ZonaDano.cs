using System.Collections;
using UnityEngine;

/// <summary>
/// ZONA DE DAÑO para los huecos sin piso. Cuando la niña cae dentro:
/// 1) pierde vida, y 2) vuelve al ultimo lugar seguro donde estuvo parada.
/// Va en un objeto con Collider2D (Is Trigger) colocado dentro del hueco.
/// </summary>
public class ZonaDano : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;
    public float cooldown = 0.6f;

    [Header("Volver al punto seguro")]
    public bool returnToSafeSpot = true;
    public float returnDelay = 0.25f;
    [Tooltip("Cuanto se sube a la niña sobre el punto seguro para que no quede metida en el piso.")]
    public float liftOnReturn = 0.3f;

    float readyTime;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void Start()
    {
        // Se asegura de que la niña tenga el "recordador" del ultimo punto seguro.
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null && p.GetComponent<SafeSpotTracker>() == null)
            p.AddComponent<SafeSpotTracker>();
    }

    void OnTriggerEnter2D(Collider2D other) { Hit(other); }
    void OnTriggerStay2D(Collider2D other) { Hit(other); }

    void Hit(Collider2D other)
    {
        if (Time.time < readyTime) return;
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        readyTime = Time.time + cooldown;
        health.TakeDamage(damage, transform.position, false);

        if (returnToSafeSpot)
            StartCoroutine(ReturnToSafe(health.gameObject));
    }

    IEnumerator ReturnToSafe(GameObject player)
    {
        yield return new WaitForSeconds(returnDelay);

        SafeSpotTracker tracker = player.GetComponent<SafeSpotTracker>();
        if (tracker == null || !tracker.hasSpot) yield break;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 target = tracker.safePosition + Vector2.up * liftOnReturn;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = target;
        }
        player.transform.position = new Vector3(target.x, target.y, player.transform.position.z);
        Physics2D.SyncTransforms();
    }
}

/// <summary>
/// Recuerda el ultimo lugar donde la niña estuvo parada en suelo firme.
/// Lo agrega solo ZonaDano; no hace falta ponerlo a mano.
/// </summary>
public class SafeSpotTracker : MonoBehaviour
{
    public Vector2 safePosition;
    public bool hasSpot;

    PlayerController2D controller;
    float groundedTime;

    void Awake()
    {
        controller = GetComponent<PlayerController2D>();
    }

    void Update()
    {
        if (controller == null) return;

        if (controller.IsGrounded)
        {
            groundedTime += Time.deltaTime;
            // Solo guarda el punto cuando lleva un ratito firme, para no guardar el borde del hueco.
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
