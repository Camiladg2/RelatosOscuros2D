using System.Collections;
using UnityEngine;

public class VidaEnemigo : MonoBehaviour, IDamageable, IWeaknessReceiver
{
    [Header("Configuracion de Salud")]
    public int vidaMaxima = 3;

    [Header("Debilidad")]
    public WeaknessType debilidad = WeaknessType.None;
    public int danoPorDebilidad = 1;
    public int danoPorOtroObjeto = 1;

    [Header("Color al recibir dano")]
    public Color colorDano = new Color(1f, 0.25f, 0.25f, 1f);
    public float duracionColor = 0.5f;

    private int vidaActual;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool estaMuerto = false;
    private SpriteRenderer[] sprites;
    private Color[] coloresOriginales;
    private Coroutine rutinaColor;

    public int VidaActual { get { return vidaActual; } }
    public bool EstaMuerto { get { return estaMuerto; } }

    private void Awake()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        sprites = GetComponentsInChildren<SpriteRenderer>();
        coloresOriginales = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) coloresOriginales[i] = sprites[i].color;
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        RecibirDano(amount);
    }

    public void ApplyWeakness(WeaknessType type, Vector2 sourcePosition)
    {
        if (type == WeaknessType.None) return;
        bool coincide = debilidad == WeaknessType.None || debilidad == type;
        RecibirDano(coincide ? danoPorDebilidad : danoPorOtroObjeto);
    }

    public void RecibirDaño(int cantidadDano)
    {
        RecibirDano(cantidadDano);
    }

    public void RecibirDano(int cantidadDano)
    {
        if (estaMuerto || cantidadDano <= 0) return;

        vidaActual -= cantidadDano;
        Debug.Log(gameObject.name + " recibio dano. Vida restante: " + vidaActual);

        if (rutinaColor != null) StopCoroutine(rutinaColor);
        rutinaColor = StartCoroutine(CambiarColor());

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            if (animator != null) animator.SetTrigger("Hurt");
        }
    }

    private IEnumerator CambiarColor()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null) sprites[i].color = colorDano;
        }

        yield return new WaitForSeconds(duracionColor);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null) sprites[i].color = coloresOriginales[i];
        }
        rutinaColor = null;
    }

    private void Morir()
    {
        estaMuerto = true;

        if (animator != null) animator.SetTrigger("Die");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.5f);
    }
}