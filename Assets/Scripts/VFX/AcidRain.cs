using System;
using UnityEngine;

public class AcidRain : MonoBehaviour
{
    public enum Phase { Calm, Warning, Rain }

    [Header("Ciclo (segundos)")]
    public float calmTime = 8f;
    public float warningTime = 2f;
    public float rainTime = 6f;
    public bool startWithRain = false;

    [Header("Dano")]
    public int damage = 1;
    [Tooltip("Cada cuantos segundos hace dano durante la lluvia. Debe ser mayor que el tiempo de invulnerabilidad de la protagonista (1.5 s).")]
    public float damageInterval = 2f;

    [Header("Zona donde cae la lluvia")]
    [Tooltip("Collider2D (Is Trigger) que cubre el area de la lluvia. Solo hace dano dentro de el. Si lo dejas vacio, hace dano en todo el nivel.")]
    public Collider2D zona;

    [Header("Refugio (techos)")]
    [Tooltip("Capas que cuentan como techo (normalmente Ground).")]
    public LayerMask coverLayer;
    [Tooltip("Hasta que altura sobre la protagonista se busca un techo.")]
    public float coverCheckHeight = 8f;
    [Tooltip("Desde que altura sobre su centro se busca el techo (para no chocar con su propio collider).")]
    public float headOffset = 1f;

    [Header("Diagnostico")]
    public bool mostrarMensajes = true;

    [Header("Efectos visuales (objetos que se encienden y apagan)")]
    public GameObject rainEffect;
    [Tooltip("Opcional: llovizna o sonido de aviso antes de que empiece la lluvia.")]
    public GameObject warningEffect;

    public Phase Current { get; private set; }
    public float TimeLeftInPhase { get; private set; }
    public bool IsRaining { get { return Current == Phase.Rain; } }

    public event Action OnWarning;
    public event Action OnRainStart;
    public event Action OnRainEnd;

    private Transform player;
    private PlayerHealth health;
    private float damageTimer;

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            health = go.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("[Lluvia de acido] No encontre al Jugador. Revisa que tenga el Tag Player.");
        }

        if (zona == null)
            Debug.LogWarning("[Lluvia de acido] No hay Zona asignada: hace dano en todo el nivel.", this);

        SetPhase(startWithRain ? Phase.Rain : Phase.Calm);
    }

    private void Update()
    {
        TimeLeftInPhase -= Time.deltaTime;
        if (TimeLeftInPhase <= 0f)
        {
            if (Current == Phase.Calm) SetPhase(Phase.Warning);
            else if (Current == Phase.Warning) SetPhase(Phase.Rain);
            else SetPhase(Phase.Calm);
        }

        if (Current == Phase.Rain) DamageTick();
    }

    private void SetPhase(Phase phase)
    {
        Phase old = Current;
        Current = phase;
        switch (phase)
        {
            case Phase.Calm: TimeLeftInPhase = calmTime; break;
            case Phase.Warning: TimeLeftInPhase = warningTime; break;
            case Phase.Rain: TimeLeftInPhase = rainTime; damageTimer = damageInterval * 0.5f; break;
        }

        if (rainEffect != null) rainEffect.SetActive(phase == Phase.Rain);
        if (warningEffect != null) warningEffect.SetActive(phase == Phase.Warning);

        if (phase == Phase.Warning && OnWarning != null) OnWarning();
        if (phase == Phase.Rain && OnRainStart != null) OnRainStart();
        if (old == Phase.Rain && phase != Phase.Rain && OnRainEnd != null) OnRainEnd();
    }

    private bool InsideZone()
    {
        if (zona == null) return true;
        Collider2D body = player.GetComponentInChildren<Collider2D>();
        if (body == null) return zona.OverlapPoint(player.position);
        return zona.Distance(body).isOverlapped;
    }

    private void DamageTick()
    {
        if (player == null || health == null)
        {
            if (mostrarMensajes) Debug.LogWarning("[Lluvia] Falta el Jugador o su PlayerHealth. player=" + (player != null) + " health=" + (health != null), this);
            return;
        }

        damageTimer -= Time.deltaTime;
        if (damageTimer > 0f) return;
        damageTimer = damageInterval;

        bool dentro = InsideZone();
        bool expuesta = IsExposed();
        if (mostrarMensajes) Debug.Log("[Lluvia] Intento de dano. Dentro de la zona: " + dentro + " | Sin techo: " + expuesta + " | Invulnerable: " + health.IsInvulnerable, this);

        if (!dentro) return;
        if (!expuesta) return;
        bool hizo = health.TakeDamage(damage, new Vector2(player.position.x, player.position.y + 5f), false);
        if (mostrarMensajes) Debug.Log("[Lluvia] Dano aplicado: " + hizo, this);
    }

    public bool IsExposed()
    {
        if (player == null) return false;
        Vector2 origin = new Vector2(player.position.x, player.position.y + headOffset);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, coverCheckHeight, coverLayer);
        return hit.collider == null;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(player.position.x, player.position.y + headOffset, 0f), 0.1f);
    }
}