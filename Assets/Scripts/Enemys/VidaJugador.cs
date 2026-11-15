using UnityEngine;
using UnityEngine.SceneManagement; // 1. IMPORTANTE: Necesario para cambiar de escena

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int vidaActual = 3;
    public int vidasRestantes = 3; // Las 3 oportunidades totales

    [Header("Escena de Game Over")]
    public string nombreEscenaGameOver = "Game_Over"; // Escribe aquí el nombre exacto de tu escena de Game Over

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("¡El jugador recibió daño! Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            ManejarMuerte();
        }
    }

    void ManejarMuerte()
    {
        vidasRestantes--; // Restamos una oportunidad

        if (vidasRestantes > 0)
        {
            // Aún tiene reapariciones
            Debug.Log("Jugador muerto. Reviviendo... Te quedan " + vidasRestantes + " oportunidades.");
            Respawn();
        }
        else
        {
            // Se acabaron las oportunidades: Carga la escena de Game Over
            PlayerPrefs.SetString("UltimaEscena", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            Debug.Log("Se acabaron las vidas. Cambiando a la escena de Game Over.");
            SceneManager.LoadScene(nombreEscenaGameOver);
        }
    }

    void Respawn()
    {
        vidaActual = 3;
        transform.position = posicionInicial;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}