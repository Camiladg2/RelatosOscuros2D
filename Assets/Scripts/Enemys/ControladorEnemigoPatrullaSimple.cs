using UnityEngine;

public class ControladorEnemigoPatrullaSimple : MonoBehaviour
{
    [Header("Patrullaje (Punto A y Punto B)")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidadPatrulla = 1.5f;
    public float tiempoEsperaEnPunto = 1f; // Pausa breve al llegar a cada extremo

    [Header("Persecución y Detección")]
    public float velocidadPersecucion = 3f;
    public float distanciaDeDeteccion = 5f;
    public float distanciaMaximaPersecucion = 10f;
    public Transform jugador;

    [Header("Daño")]
    public int dañoAlJugador = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform objetivoActual;
    private bool estaMuerto = false;
    private bool estaPersiguiendo = false;
    private float contadorEspera = 0f;
    private Vector3 escalaOriginal;
    private float posicionZOriginal;
    private VidaEnemigo scriptVida;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
        posicionZOriginal = transform.position.z;
        scriptVida = GetComponent<VidaEnemigo>();

        // Empieza caminando hacia el Punto B
        objetivoActual = puntoB;

        if (jugador == null)
        {
            GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
            if (objJugador != null) jugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (jugador == null) return;
        if (scriptVida != null && scriptVida.vidaMaxima <= 0) return; // Opcional

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        // Revisar si el jugador está dentro del rango de visión
        if (distanciaAlJugador <= distanciaDeDeteccion)
        {
            estaPersiguiendo = true;
        }
        else if (distanciaAlJugador > distanciaMaximaPersecucion)
        {
            estaPersiguiendo = false;
        }

        if (estaPersiguiendo)
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        if (puntoA == null || puntoB == null) return;

        // Distancia horizontal hacia el punto destino actual
        float distanciaAlObjetivo = Mathf.Abs(transform.position.x - objetivoActual.position.x);

        if (distanciaAlObjetivo < 0.2f)
        {
            // Llegó al punto: se detiene un momento antes de dar la vuelta
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);

            contadorEspera += Time.deltaTime;
            if (contadorEspera >= tiempoEsperaEnPunto)
            {
                // Cambia al otro punto
                objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
                contadorEspera = 0f;
            }
        }
        else
        {
            // Camina hacia el punto destino
            int direccion = (objetivoActual.position.x > transform.position.x) ? 1 : -1;
            Mover(direccion, velocidadPatrulla);
        }
    }

    void PerseguirJugador()
    {
        int direccion = (jugador.position.x > transform.position.x) ? 1 : -1;
        Mover(direccion, velocidadPersecucion);
    }

    void Mover(int direccion, float velocidadActual)
    {
        rb.linearVelocity = new Vector2(direccion * velocidadActual, rb.linearVelocity.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, posicionZOriginal);
        animator.SetFloat("Speed", Mathf.Abs(velocidadActual));

        // Voltear sprite manteniendo escala
        if (direccion > 0)
            transform.localScale = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(-1 * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");

            VidaJugador scriptVidaJugador = collision.gameObject.GetComponent<VidaJugador>();
            if (scriptVidaJugador != null)
            {
                scriptVidaJugador.RecibirDaño(dañoAlJugador);
            }
        }
    }

    // Dibuja los Gizmos de visión y la línea entre Punto A y Punto B
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