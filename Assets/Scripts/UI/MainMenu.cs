using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuOpcionesGO;
    public GameObject mainMenuGO;
    public GameObject menuCreditosGO; // <--- Descomentado para usarlo con los créditos

    public void OpenOptionsPanel()
    {
        if (mainMenuGO != null) mainMenuGO.SetActive(false);
        if (menuOpcionesGO != null) menuOpcionesGO.SetActive(true);
        if (menuCreditosGO != null) menuCreditosGO.SetActive(false); // Asegura que los créditos estén apagados
    }

    // NUEVA FUNCIÓN: Abre el panel de créditos y oculta el menú principal
    public void OpenCreditsPanel()
    {
        if (mainMenuGO != null) mainMenuGO.SetActive(false);
        if (menuOpcionesGO != null) menuOpcionesGO.SetActive(false);
        if (menuCreditosGO != null) menuCreditosGO.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        if (mainMenuGO != null) mainMenuGO.SetActive(true);
        if (menuOpcionesGO != null) menuOpcionesGO.SetActive(false);
        if (menuCreditosGO != null) menuCreditosGO.SetActive(false); // Apaga los créditos al volver
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // BOTÓN JUGAR / CONTINUAR: Carga la última escena donde estuvo el jugador
    public void PlayGame()
    {
        string escenaGuardada = PlayerPrefs.GetString("UltimaEscena", "World_1");
        SceneManager.LoadScene(escenaGuardada);
    }

    // BOTÓN NUEVA PARTIDA: Borra el progreso anterior y empieza desde cero en World_1
    public void NewGame()
    {
        PlayerPrefs.DeleteAll(); // Borra el progreso anterior
        PlayerPrefs.SetString("UltimaEscena", "World_1");
        PlayerPrefs.SetInt("PocionesGuardadas", 0);
        PlayerPrefs.Save(); // Fuerza el guardado en disco

        SceneManager.LoadScene("World_1");
    }
}