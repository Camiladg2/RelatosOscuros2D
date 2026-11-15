using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private bool estaPausado = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (pauseButton == null)
        {
            Transform btnTransform = transform.Find("Btn_Pausa");
            if (btnTransform == null) btnTransform = transform.Find("Canvas/Btn_Pausa");
            if (btnTransform != null) pauseButton = btnTransform.gameObject;
        }

        if (pauseMenu == null)
        {
            Transform panelTransform = transform.Find("Pause_Panel");
            if (panelTransform == null) panelTransform = transform.Find("Canvas/Pause_Panel");
            if (panelTransform != null) pauseMenu = panelTransform.gameObject;
        }

        estaPausado = false;
        if (pauseButton != null) pauseButton.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    private void Update()
    {
        // Detecta la tecla Escape (Esc) para pausar o reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        if (pauseButton != null) pauseButton.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Debug.Log("Juego Pausado");
    }

    public void ResumeGame()
    {
        estaPausado = false;
        Time.timeScale = 1f;
        if (pauseButton != null) pauseButton.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Debug.Log("Juego Reanudado");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimaEscena", escenaActual);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Inicio");
    }

    public void QuitGame()
    {
        Debug.Log("Close Game");
        Application.Quit();
    }
}
