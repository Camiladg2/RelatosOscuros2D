using UnityEngine;
using UnityEngine.UI;

public class VidaHUD : MonoBehaviour
{
    [Header("Corazones (de izquierda a derecha)")]
    public Image[] corazones;
    public Sprite corazonLleno;
    public Sprite corazonMedio;
    public Sprite corazonVacio;
    public Color colorLleno = Color.white;
    public Color colorVacio = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Header("Cuantos puntos de vida vale cada corazon")]
    [Tooltip("2 = con medio corazon (Max Health 6 para 3 corazones). 1 = sin medio corazon (Max Health 3).")]
    public int puntosPorCorazon = 2;

    [Header("Vidas restantes (opcional)")]
    public Text textoVidas;
    public string prefijoVidas = "Vidas: ";

    private PlayerHealth health;
    private int ultimasVidas = -999;

    private void OnEnable()
    {
        Conectar();
    }

    private void OnDisable()
    {
        if (health != null) health.OnHealthChanged -= Actualizar;
        health = null;
    }

    private void Conectar()
    {
        if (health != null) return;

        GameObject p = GameObject.FindWithTag("Player");
        if (p == null) return;

        health = p.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.OnHealthChanged += Actualizar;
        Actualizar(health.CurrentHealth, health.maxHealth);
    }

    private void Update()
    {
        if (health == null) Conectar();

        if (textoVidas != null && RespawnManager.Instance != null)
        {
            int v = RespawnManager.Instance.Vidas;
            if (v != ultimasVidas)
            {
                ultimasVidas = v;
                textoVidas.text = prefijoVidas + Mathf.Max(0, v);
            }
        }
    }

    public void Actualizar(int actual, int maxima)
    {
        if (corazones == null) return;

        int pp = Mathf.Max(1, puntosPorCorazon);

        for (int i = 0; i < corazones.Length; i++)
        {
            Image c = corazones[i];
            if (c == null) continue;

            c.gameObject.SetActive(i * pp < maxima);

            int puntos = Mathf.Clamp(actual - i * pp, 0, pp);

            Sprite elegido;
            if (puntos >= pp) elegido = corazonLleno;
            else if (puntos > 0 && corazonMedio != null) elegido = corazonMedio;
            else elegido = corazonVacio;

            if (corazonLleno != null && corazonVacio != null)
            {
                c.sprite = elegido;
                c.color = Color.white;
            }
            else
            {
                c.color = puntos > 0 ? colorLleno : colorVacio;
            }
        }
    }
}