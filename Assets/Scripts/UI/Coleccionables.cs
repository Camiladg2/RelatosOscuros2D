using UnityEngine;
using TMPro;

public class ContadorColeccionables : MonoBehaviour
{
    [Header("Configuración del Contador")]
    public int coleccionablesRecogidos = 0;

    [Header("Componentes de Interfaz")]
    public TextMeshProUGUI textoContador;

    void Start()
    {
        ActualizarInterfaz();
    }

    // detector para juegos 3D
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coleccionable"))
        {
            Recolectar(other.gameObject);
        }
    }

    // detector para juegos 2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coleccionable"))
        {
            Recolectar(other.gameObject);
        }
    }

    void Recolectar(GameObject objetoGema)
    {
        coleccionablesRecogidos++;
        ActualizarInterfaz();
        Destroy(objetoGema);
    }

    void ActualizarInterfaz()
    {
        if (textoContador != null)
        {
            textoContador.text = "Pociones: " + coleccionablesRecogidos;
        }
    }
}
