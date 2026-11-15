using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorDeAudio : MonoBehaviour
{
    public static GestorDeAudio Instance;

    [Header("Fuentes de Audio")]
    public AudioSource musicaSource;
    public AudioSource sfxSource;

    [Header("Clips de Música")]
    public AudioClip musicaInicio;
    public AudioClip musicaCreditos; // <--- 1. NUEVA CANCIÓN PARA CRÉDITOS
    public AudioClip musicaMundo1;
    public AudioClip musicaMundo2;
    public AudioClip musicaMundo3;
    public AudioClip musicaGameOver;

    [Header("Clips de Efectos (SFX)")]
    public AudioClip sonidoClicBotones;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string nombreEscena = scene.name;

        if (nombreEscena == "Inicio" || nombreEscena == "Final")
        {
            CambiarMusica(musicaInicio);
        }
        else if (nombreEscena == "World_1")
        {
            CambiarMusica(musicaMundo1);
        }
        else if (nombreEscena == "World_2")
        {
            CambiarMusica(musicaMundo2);
        }
        else if (nombreEscena == "World_3")
        {
            CambiarMusica(musicaMundo3);
        }
        else if (nombreEscena == "Game_Over")
        {
            CambiarMusica(musicaGameOver);
        }
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (musicaSource == null || nuevaMusica == null) return;
        if (musicaSource.clip == nuevaMusica && musicaSource.isPlaying) return;

        musicaSource.clip = nuevaMusica;
        musicaSource.loop = true;
        musicaSource.Play();
    }

    // --- FUNCIONES EXTRAS PARA LOS CRÉDITOS ---
    public void ReproducirMusicaCreditos()
    {
        CambiarMusica(musicaCreditos);
    }

    public void ReproducirMusicaInicio()
    {
        CambiarMusica(musicaInicio);
    }
    // ------------------------------------------

    public void ReproducirSonidoClic()
    {
        if (sfxSource != null && sonidoClicBotones != null)
        {
            sfxSource.PlayOneShot(sonidoClicBotones);
        }
    }

    [Header("Audio Extra Hover")]
    public AudioClip sonidoHoverBotones; // Arrastra aquí un sonido más suave

    public void ReproducirSonidoHover()
    {
        if (sfxSource != null && sonidoHoverBotones != null)
        {
            sfxSource.PlayOneShot(sonidoHoverBotones);
        }
    }
}