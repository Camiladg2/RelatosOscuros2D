using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    public PlayerHealth jugador;
    public int vidasMaximas = 3;
    public float esperaAntesDeReaparecer = 1.5f;
    public float esperaDespuesDeCaer = 0.3f;
    public float profundidadMaxima = 30f;

    [Header("Pantalla negra (opcional)")]
    public CanvasGroup pantallaNegra;
    public float tiempoFundido = 0.4f;

    [Header("Game Over")]
    public string escenaGameOver = "Game_Over";

    [Header("Eventos para la interfaz")]
    public UnityEvent<int> alCambiarVidas;
    public UnityEvent alReaparecer;
    public UnityEvent alGameOver;

    private static int vidas = -1;
    private static bool tieneCheckpoint;
    private static Vector3 posCheckpoint;
    private static string escenaCheckpoint = "";
    private static bool cargarGuardado;

    private Vector3 posInicial;
    private bool enGameOver;

    public int Vidas { get { return vidas; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) jugador = p.GetComponent<PlayerHealth>();
        }
        if (jugador == null)
        {
            Debug.LogError("[Respawn] No encuentro a la jugadora. Arrastrala a la casilla Jugador o ponle el Tag Player.", this);
            enabled = false;
            return;
        }

        posInicial = jugador.transform.position;
        string escena = SceneManager.GetActiveScene().name;
        if (escenaCheckpoint != escena) tieneCheckpoint = false;
        if (vidas < 0) vidas = vidasMaximas;

        if (cargarGuardado)
        {
            cargarGuardado = false;
            string escenaGuardada;
            Vector3 posGuardada;
            if (PartidaGuardada.Cargar(out escenaGuardada, out posGuardada) && escenaGuardada == escena)
            {
                vidas = vidasMaximas;
                tieneCheckpoint = true;
                escenaCheckpoint = escena;
                posCheckpoint = posGuardada;
                jugador.transform.position = posGuardada;
                Physics2D.SyncTransforms();
            }
        }

        if (pantallaNegra != null)
        {
            pantallaNegra.alpha = 0f;
            pantallaNegra.blocksRaycasts = false;
        }

        jugador.OnDied += AlMorir;
        alCambiarVidas.Invoke(vidas);
    }

    private void OnDestroy()
    {
        if (jugador != null) jugador.OnDied -= AlMorir;
        if (Instance == this) Instance = null;
    }

    private void AlMorir()
    {
        if (enGameOver) return;

        vidas = Mathf.Max(0, vidas - 1);
        alCambiarVidas.Invoke(vidas);

        if (vidas <= 0)
        {
            enGameOver = true;
            StartCoroutine(RutinaGameOver());
            return;
        }
        float espera = jugador.MurioPorCaida ? esperaDespuesDeCaer : esperaAntesDeReaparecer;
        StartCoroutine(RutinaReaparecer(espera));
    }

    private void Update()
    {
        if (jugador == null || jugador.IsDead || enGameOver) return;
        if (profundidadMaxima <= 0f) return;

        if (jugador.transform.position.y < posInicial.y - profundidadMaxima)
            jugador.Kill();
    }

    private IEnumerator RutinaGameOver()
    {
        yield return new WaitForSeconds(esperaAntesDeReaparecer);
        alGameOver.Invoke();

        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimaEscena", escenaActual);
        PlayerPrefs.Save();

        vidas = -1;

        if (!string.IsNullOrEmpty(escenaGameOver))
        {
            if (Application.CanStreamedLevelBeLoaded(escenaGameOver))
                SceneManager.LoadScene(escenaGameOver);
            else
                Debug.LogError("[Respawn] La escena '" + escenaGameOver + "' no esta en Build Profiles > Scene List, o el nombre no coincide.", this);
        }
    }

    private IEnumerator RutinaReaparecer(float espera)
    {
        yield return new WaitForSeconds(espera);
        yield return Fundido(0f, 1f);
        jugador.Respawn(PosicionDeReaparicion());
        alReaparecer.Invoke();
        yield return Fundido(1f, 0f);
    }

    private Vector3 PosicionDeReaparicion()
    {
        return tieneCheckpoint ? posCheckpoint : posInicial;
    }

    private IEnumerator Fundido(float desde, float hasta)
    {
        if (pantallaNegra == null || tiempoFundido <= 0f) yield break;

        pantallaNegra.blocksRaycasts = true;
        float t = 0f;
        while (t < tiempoFundido)
        {
            t += Time.unscaledDeltaTime;
            pantallaNegra.alpha = Mathf.Lerp(desde, hasta, Mathf.Clamp01(t / tiempoFundido));
            yield return null;
        }
        pantallaNegra.alpha = hasta;
        pantallaNegra.blocksRaycasts = hasta > 0.5f;
    }

    public void ActivarCheckpoint(Vector3 posicion)
    {
        tieneCheckpoint = true;
        posCheckpoint = posicion;
        escenaCheckpoint = SceneManager.GetActiveScene().name;
        PartidaGuardada.Guardar(escenaCheckpoint, posicion);
    }

    public void Reintentar()
    {
        enGameOver = false;
        vidas = vidasMaximas;
        alCambiarVidas.Invoke(vidas);
        StopAllCoroutines();
        StartCoroutine(RutinaReaparecer(esperaAntesDeReaparecer));
    }

    public void ReiniciarVidas()
    {
        enGameOver = false;
        vidas = vidasMaximas;
        alCambiarVidas.Invoke(vidas);
    }

    public static void NuevaPartida(string escenaInicial)
    {
        PartidaGuardada.Borrar();
        GameProgress.ResetAll();
        vidas = -1;
        tieneCheckpoint = false;
        escenaCheckpoint = "";
        cargarGuardado = false;
        SceneManager.LoadScene(escenaInicial);
    }

    public static bool HayPartidaGuardada
    {
        get { return PartidaGuardada.Existe; }
    }

    public static void Continuar()
    {
        string escena;
        Vector3 pos;
        if (!PartidaGuardada.Cargar(out escena, out pos)) return;
        vidas = -1;
        cargarGuardado = true;
        SceneManager.LoadScene(escena);
    }
}

public static class PartidaGuardada
{
    private const string KEscena = "RO_escena";
    private const string KX = "RO_x";
    private const string KY = "RO_y";

    public static bool Existe
    {
        get { return PlayerPrefs.HasKey(KEscena); }
    }

    public static void Guardar(string escena, Vector3 posicion)
    {
        PlayerPrefs.SetString(KEscena, escena);
        PlayerPrefs.SetFloat(KX, posicion.x);
        PlayerPrefs.SetFloat(KY, posicion.y);
        PlayerPrefs.Save();
    }

    public static bool Cargar(out string escena, out Vector3 posicion)
    {
        escena = "";
        posicion = Vector3.zero;
        if (!Existe) return false;
        escena = PlayerPrefs.GetString(KEscena);
        posicion = new Vector3(PlayerPrefs.GetFloat(KX), PlayerPrefs.GetFloat(KY), 0f);
        return true;
    }

    public static void Borrar()
    {
        PlayerPrefs.DeleteKey(KEscena);
        PlayerPrefs.DeleteKey(KX);
        PlayerPrefs.DeleteKey(KY);
        PlayerPrefs.Save();
    }
}