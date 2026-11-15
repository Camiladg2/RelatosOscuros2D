using UnityEngine;

public class ControladorEnemigo : MonoBehaviour
{
    [Header("Movimiento y Detección")]
    public float velocidad = 2.5f;
    public float fuerzaSalto = 5f;
    public float distanciaDeDeteccion = 6f;
    public Transform jugador;

    [Header("Daño")]
    public int dañoAlJugador = 1;

    [Header("Detección de Plataformas / Abismos")]
    public LayerMask capaSuelo;
    public float distanciaSensorAdelante = 0.6f; // Qué tan adelante se coloca el sensor
    public float alturaSensorY = -0.5f;        // Qué tan abajo (en los pies) se coloca
    public float radioDeteccion = 0.15f;
    public float tiempoEntreSaltos = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool estaMuerto = false;
    private bool estaPersiguiendo = false;
    private Vector3 escalaOriginal;
    private float posicionZOriginal;
    private float tiempoSiguienteSalto = 0f;
    private VidaEnemigo scriptVida;

    // Posición calculada del sensor en tiempo real
    private Vector2 posicionSensorActual;

    void Start()
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

    void Update()
    {
        if (jugador == null) return;
        if (scriptVida != null && scriptVida.vidaMaxima <= 0) return; // Opcional

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaDeDeteccion || estaPersiguiendo)
        {
            estaPersiguiendo = true;
            MoverEnPlataformas();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0f);
        }
    }

    void MoverEnPlataformas()
    {
        int direccion = (jugador.position.x > transform.position.x) ? 1 : -1;

        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, posicionZOriginal);

        animator.SetFloat("Speed", Mathf.Abs(velocidad));

        // Voltear sprite manteniendo la escala original
        if (direccion > 0)
        {
            transform.localScale = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }
        else
        {
            transform.localScale = new Vector3(-1 * escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }

        // --- CALCULAR EL SENSOR AUTOMÁTICAMENTE AL FRENTE (IZQUIERDA O DERECHA) ---
        // Tomamos la posición de la Llorona y le sumamos la distancia hacia adelante según su dirección
        float posXSensor = transform.position.x + (distanciaSensorAdelante * direccion);
        float posYSensor = transform.position.y + alturaSensorY;
        posicionSensorActual = new Vector2(posXSensor, posYSensor);

        // Detecta si hay suelo justo delante de los pies en esa posición calculada
        bool haySueloAdelante = Physics2D.OverlapCircle(posicionSensorActual, radioDeteccion, capaSuelo);

        // Si hay un abismo (no hay suelo adelante), está en el suelo y ya pasó el tiempo: SALTA
        if (!haySueloAdelante && Mathf.Abs(rb.linearVelocity.y) < 0.05f && Time.time >= tiempoSiguienteSalto)
        {
            Saltar();
            tiempoSiguienteSalto = Time.time + tiempoEntreSaltos;
        }
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
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

    // Dibuja los Gizmos para ver el radio de detección y el sensor flotante calculado por código
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeDeteccion);

        // Dibuja el sensor de los pies en tiempo real en la escena
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicionSensorActual, radioDeteccion);
    }
}