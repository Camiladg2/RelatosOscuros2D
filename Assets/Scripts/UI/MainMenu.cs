using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // En Unity arrastra aquí tu objeto "MenuOpciones"
    public GameObject menuOpcionesGO;

    // En Unity arrastra aquí tu objeto "MainMenu"
    public GameObject mainMenuGO;

    // REEMPLAZAR AQUÍ: Si tienes un panel para los créditos, descomenta la línea de abajo borrando las dos barras (//)
    // public GameObject menuCreditosGO;

    public void OpenOptionsPanel()
    {
        mainMenuGO.SetActive(false);
        menuOpcionesGO.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenuGO.SetActive(true);
        menuOpcionesGO.SetActive(false);

        // Si activaste el panel de créditos arriba, descomenta esta línea también:
        // if(menuCreditosGO != null) menuCreditosGO.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        // Carga la escena de continuar o la última guardada
        SceneManager.LoadScene("World_1");
    }

    // 1. NUEVA FUNCIÓN: NUEVA PARTIDA
    public void NewGame()
    {

        SceneManager.LoadScene("World_1");

        // NOTA: Si usas un sistema de guardado (PlayerPrefs), aquí deberías reiniciarlo antes de cargar la escena
        // Ejemplo: PlayerPrefs.DeleteAll();
    }

    // 2. NUEVA FUNCIÓN: CRÉDITOS
    public void OpenCredits()
    {
        // Opción A: Si los créditos son otra escena entera
        // SceneManager.LoadScene("EscenaCreditos");

        // Opción B: Si los créditos son un panel dentro del mismo Canvas (Igual que las opciones)
        // Para usarlo, debes descomentar las variables de arriba y usar esto:
        // mainMenuGO.SetActive(false);
        // menuCreditosGO.SetActive(true);

        Debug.Log("Abriendo Créditos..."); // Esto mostrará un mensaje en la consola de Unity para probar que funciona
    }
}
