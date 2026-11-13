using UnityEngine;
using UnityEngine.Events;

/// <summary>Detecta lo que hay cerca de la protagonista y lo activa al presionar E.</summary>
public class PlayerInteractor : MonoBehaviour
{
    public float radius = 1.0f;
    [Tooltip("Punto de busqueda respecto a la protagonista (se voltea solo cuando ella mira a la izquierda).")]
    public Vector2 offset = new Vector2(0.6f, 0f);
    public LayerMask interactLayer = ~0;

    private PlayerController2D controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        if (controller != null && !controller.CanControl) return;
        if (!GameInput.InteractPressed()) return;

        Vector3 center = transform.TransformPoint(offset);
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, interactLayer);

        IInteractable best = null;
        float bestDist = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            IInteractable candidate = hit.GetComponentInParent<IInteractable>();
            if (candidate == null) continue;
            float d = Vector2.Distance(center, hit.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = candidate;
            }
        }
        if (best != null) best.Interact(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(offset), radius);
    }
}
