using UnityEngine;

/// <summary>
/// Planta carnivora: se queda tranquila y de vez en cuando ataca a la jugadora si esta cerca.
/// Va en el MISMO objeto que tiene el Animator y el SpriteRenderer de la planta.
/// El golpe se aplica con un Animation Event llamado Golpear() en el frame de la mordida.
/// </summary>
public class PlantaCarnivora : MonoBehaviour
{
    [Header("Cuando ataca")]
    [Tooltip("Distancia maxima a la que la planta se anima a atacar.")]
    public float rangoDeteccion = 5f;
    [Tooltip("Espera entre ataques: un numero al azar entre estos dos (segundos).")]
    public float esperaMin = 3f;
    public float esperaMax = 6f;
    [Tooltip("Nombre del Trigger en el Animator.")]
    public string triggerAtaque = "Attack";

    [Header("El golpe")]
    [Tooltip("Objeto hijo puesto donde esta la boca en el frame de la mordida (frame 08).")]
    public Transform puntoGolpe;
    public float radioGolpe = 1.2f;
    public int dano = 1;

    [Header("Opcional")]
    [Tooltip("La planta se voltea hacia la jugadora (el dibujo original mira a la derecha).")]
    public bool mirarAlJugador = true;
    [Tooltip("Escribe mensajes en la Console para ver que pasa. Desmarcalo al terminar.")]
    public bool mostrarMensajes = true;

    Animator anim;
    Transform jugador;
    float proximoAtaque;
    float ultimoAtaque = -10f;
    float escalaX;

    void Awake()
    {
        anim = GetComponent<Animator>();
        escalaX = Mathf.Abs(transform.localScale.x);
    }

    void Start()
    {
        BuscarJugador();
        Programar();
    }

    void BuscarJugador()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) jugador = p.transform;
    }

    void Programar()
    {
        proximoAtaque = Time.time + Random.Range(esperaMin, esperaMax);
    }

    void Update()
    {
        if (jugador == null) { BuscarJugador(); return; }

        // Voltearse hacia la jugadora, pero no en pleno ataque.
        if (mirarAlJugador && Time.time - ultimoAtaque > 1.5f)
        {
            float lado = jugador.position.x >= transform.position.x ? 1f : -1f;
            Vector3 s = transform.localScale;
            s.x = escalaX * lado;
            transform.localScale = s;
        }

        if (Time.time < proximoAtaque) return;

        if (Vector2.Distance(jugador.position, transform.position) > rangoDeteccion)
        {
            proximoAtaque = Time.time + 0.5f; // vuelve a mirar en medio segundo
            return;
        }

        if (anim != null) anim.SetTrigger(triggerAtaque);
        ultimoAtaque = Time.time;
        Programar();
        Log("Empieza el ataque");
    }

    /// <summary>Se llama desde un Animation Event en el frame de la mordida.</summary>
    public void Golpear()
    {
        Log("Golpear() llamado");
        Vector2 centro = puntoGolpe != null ? (Vector2)puntoGolpe.position : (Vector2)transform.position;
        Collider2D[] golpeados = Physics2D.OverlapCircleAll(centro, radioGolpe);
        Log("Cosas dentro del circulo: " + golpeados.Length);

        foreach (Collider2D c in golpeados)
        {
            PlayerHealth vida = c.GetComponentInParent<PlayerHealth>();
            if (vida == null) continue;

            // Con el poder de la savia, la planta no le hace daño.
            PlayerPowerUps poderes = c.GetComponentInParent<PlayerPowerUps>();
            if (poderes != null && poderes.IsPlantImmune)
            {
                Log("La niña tiene savia: no recibe daño");
                return;
            }

            Log("Daño a la niña");
            vida.TakeDamage(dano, centro, true);
            return;
        }

        Log("La niña NO estaba dentro del circulo");
    }

    void Log(string mensaje)
    {
        if (mostrarMensajes) Debug.Log("[Planta] " + mensaje, this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 p = puntoGolpe != null ? puntoGolpe.position : transform.position;
        Gizmos.DrawWireSphere(p, radioGolpe);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}