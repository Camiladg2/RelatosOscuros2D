using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public Transform puntoReaparicion;
    public GameObject visualActivo;
    public GameObject visualInactivo;
    public bool curarAlActivar = true;
    public UnityEvent alActivar;

    private static Checkpoint actual;
    private bool activo;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void Start()
    {
        Mostrar(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activo) return;

        PlayerHealth vida = other.GetComponentInParent<PlayerHealth>();
        if (vida == null || vida.IsDead) return;

        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[Checkpoint] Falta el RespawnManager en la escena.", this);
            return;
        }

        Vector3 pos = puntoReaparicion != null ? puntoReaparicion.position : transform.position;
        RespawnManager.Instance.ActivarCheckpoint(pos);

        if (actual != null && actual != this) actual.Mostrar(false);
        actual = this;
        Mostrar(true);

        if (curarAlActivar) vida.Heal(vida.maxHealth);
        alActivar.Invoke();
    }

    private void Mostrar(bool encendido)
    {
        activo = encendido;
        if (visualActivo != null) visualActivo.SetActive(encendido);
        if (visualInactivo != null) visualInactivo.SetActive(!encendido);
    }
}
