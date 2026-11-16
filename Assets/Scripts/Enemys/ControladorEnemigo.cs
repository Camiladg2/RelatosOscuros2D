using UnityEngine;
using UnityEngine.Serialization;

public class ControladorEnemigo : MonoBehaviour
{
    [Header("Movimiento y Deteccion")]
    public float velocidad = 2.5f;
    public float fuerzaSalto = 5f;
    public float distanciaDeDeteccion = 6f;
    public Transform jugador;

    [Header("Dano")]
    [FormerlySerializedAs("dañoAlJugador")]
    public int danoAlJugador = 1;

    [Header("Deteccion de Plataformas / Abismos")]
    public LayerMask capaSuelo;
    public float distanciaSensorAdelante = 0.6f;
    public float alturaSensorY = -0.5f;
    public float radioDeteccion = 0.15f;
    public float tiempoEntreSaltos = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool estaPersiguiendo = false;
    private Vector3 escalaOriginal;
    private float posicionZOriginal;
    private float tiempoSiguienteSalto = 0f;
    private VidaEnemigo scriptVida;
    private Vector2 posicionSensorActual;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
        posicionZOriginal = transform.position.z;
        scriptVida = GetComponent<VidaEnemigo>();

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

        if (distanciaAlJugador <= distanciaDeDeteccion || estaPersiguiendo)
        {
            estaPersiguiendo = true;
            MoverEnPlataformas();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }

    private void MoverEnPlataformas()
    {
        int direccion = (jugador.position.x > transform.position.x) ? 1 : -1;

        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, posicionZOriginal);

        if (animator != null) animator.SetFloat("Speed", Mathf.Abs(velocidad));

        if (direccion > 0)
            transform.localScale = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(-1 * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);

        float posXSensor = transform.position.x + (distanciaSensorAdelante * direccion);
        float posYSensor = transform.position.y + alturaSensorY;
        posicionSensorActual = new Vector2(posXSensor, posYSensor);

        bool haySueloAdelante = Physics2D.OverlapCircle(posicionSensorActual, radioDeteccion, capaSuelo);

        if (!haySueloAdelante && Mathf.Abs(rb.linearVelocity.y) < 0.05f && Time.time >= tiempoSiguienteSalto)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            tiempoSiguienteSalto = Time.time + tiempoEntreSaltos;
        }
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicionSensorActual, radioDeteccion);
    }
}