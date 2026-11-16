using UnityEngine;
using UnityEngine.Serialization;

public class ControladorEnemigoPatrullaSimple : MonoBehaviour
{
    [Header("Patrullaje (Punto A y Punto B)")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidadPatrulla = 1.5f;
    public float tiempoEsperaEnPunto = 1f;

    [Header("Persecucion y Deteccion")]
    public float velocidadPersecucion = 3f;
    public float distanciaDeDeteccion = 5f;
    public float distanciaMaximaPersecucion = 10f;
    public Transform jugador;

    [Header("Dano")]
    [FormerlySerializedAs("dañoAlJugador")]
    public int danoAlJugador = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform objetivoActual;
    private bool estaPersiguiendo = false;
    private float contadorEspera = 0f;
    private Vector3 escalaOriginal;
    private float posicionZOriginal;
    private VidaEnemigo scriptVida;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
        posicionZOriginal = transform.position.z;
        scriptVida = GetComponent<VidaEnemigo>();

        objetivoActual = puntoB;

        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    private void Update()
    {
        if (jugador == null) return;
        if (scriptVida != null && scriptVida.EstaMuerto) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaDeDeteccion)
            estaPersiguiendo = true;
        else if (distanciaAlJugador > distanciaMaximaPersecucion)
            estaPersiguiendo = false;

        if (estaPersiguiendo) PerseguirJugador();
        else Patrullar();
    }

    private void Patrullar()
    {
        if (puntoA == null || puntoB == null) return;

        float distanciaAlObjetivo = Mathf.Abs(transform.position.x - objetivoActual.position.x);

        if (distanciaAlObjetivo < 0.2f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null) animator.SetFloat("Speed", 0f);

            contadorEspera += Time.deltaTime;
            if (contadorEspera >= tiempoEsperaEnPunto)
            {
                objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
                contadorEspera = 0f;
            }
        }
        else
        {
            int direccion = (objetivoActual.position.x > transform.position.x) ? 1 : -1;
            Mover(direccion, velocidadPatrulla);
        }
    }

    private void PerseguirJugador()
    {
        int direccion = (jugador.position.x > transform.position.x) ? 1 : -1;
        Mover(direccion, velocidadPersecucion);
    }

    private void Mover(int direccion, float velocidadActual)
    {
        rb.linearVelocity = new Vector2(direccion * velocidadActual, rb.linearVelocity.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, posicionZOriginal);
        if (animator != null) animator.SetFloat("Speed", Mathf.Abs(velocidadActual));

        if (direccion > 0)
            transform.localScale = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(-1 * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Golpear(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Golpear(collision);
    }

    private void Golpear(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (scriptVida != null && scriptVida.EstaMuerto) return;

        PlayerHealth vida = collision.gameObject.GetComponentInParent<PlayerHealth>();
        if (vida == null) return;

        if (vida.TakeDamage(danoAlJugador, transform.position) && animator != null)
            animator.SetTrigger("Attack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeDeteccion);

        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawSphere(puntoA.position, 0.15f);
            Gizmos.DrawSphere(puntoB.position, 0.15f);
        }
    }
}