using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private void Start()
    {
        Time.timeScale = 1; // Asegura que el juego no inicie congelado

        // Buscamos los objetos dentro de este mismo Prefab para no perder las referencias al recargar
        if (pauseButton == null)
        {
            Transform btnTransform = transform.Find("Canvas/Btn_Pausa");
            if (btnTransform != null) pauseButton = btnTransform.gameObject;
        }

        if (pauseMenu == null)
        {
            Transform panelTransform = transform.Find("Canvas/Pause_Panel");
            if (panelTransform != null) pauseMenu = panelTransform.gameObject;
        }

        // Forzar los estados iniciales correctos cada vez que se recarga la escena
        if (pauseButton != null) pauseButton.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        if (pauseButton != null) pauseButton.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pauseButton != null) pauseButton.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Inicio"); // Cambiado a "inicio" que es el nombre que veo en tus Assets
    }

    public void QuitGame()
    {
        Debug.Log("Close Game");
        Application.Quit();
    }
}
