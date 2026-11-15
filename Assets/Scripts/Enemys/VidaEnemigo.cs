using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int vidaMaxima = 3; // Cambia este número en el Inspector para cada enemigo
    private int vidaActual;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool estaMuerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Esta función la llamará el ataque de tu jugador
    public void RecibirDaño(int cantidadDaño)
    {
        if (estaMuerto) return;

        vidaActual -= cantidadDaño;
        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            // Activa la animación de daño si aún sigue vivo
            if (animator != null) animator.SetTrigger("Hurt");
        }
    }

    void Morir()
    {
        estaMuerto = true;

        // 1. Activa la animación de muerte
        if (animator != null) animator.SetTrigger("Die");

        // 2. Detiene su movimiento físico y desactiva colisiones para que no siga haciendo daño
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // Evita que caiga o empuje al jugador mientras desaparece
        }
        if (col != null) col.enabled = false;

        // 3. Destruye el objeto después de 1.5 segundos (tiempo para que se vea la animación Die)
        Destroy(gameObject, 1.5f);
    }
}