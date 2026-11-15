using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Función para el botón "Menú Principal"
    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1; // Asegura que el tiempo corra normal
        SceneManager.LoadScene("Inicio");
    }

    // Función extra por si tienes un botón de "Reintentar"
    public void ReintentarNivel()
    {
        Time.timeScale = 1;
        string escenaGuardada = PlayerPrefs.GetString("UltimaEscena", "World_1");
        SceneManager.LoadScene(escenaGuardada);
    }
}