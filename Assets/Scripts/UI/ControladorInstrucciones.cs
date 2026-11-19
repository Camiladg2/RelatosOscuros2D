using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorInstrucciones : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelInstrucciones; // Arrastra aquí tu PanelInstrucciones
    public float tiempoEnPantalla = 4f;   // Cuántos segundos se mostrará la imagen

    // Método que puedes llamar directamente desde el botón "Jugar" de tu menú
    public void JugarConInstrucciones(string nombreEscenaMundo1)
    {
        StartCoroutine(MostrarInstruccionesYJugar(nombreEscenaMundo1));
    }

    private IEnumerator MostrarInstruccionesYJugar(string nombreEscena)
    {
        // 1. Activa el panel en pantalla
        if (panelInstrucciones != null)
        {
            panelInstrucciones.SetActive(true);
        }

        // 2. Espera los segundos configurados (ej. 4 segundos)
        yield return new WaitForSeconds(tiempoEnPantalla);

        // 3. Oculta el panel y carga la escena del juego
        if (panelInstrucciones != null)
        {
            panelInstrucciones.SetActive(false);
        }

        SceneManager.LoadScene(nombreEscena);
    }
}