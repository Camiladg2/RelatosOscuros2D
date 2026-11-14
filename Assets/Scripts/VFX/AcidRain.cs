using System;
using UnityEngine;

/// <summary>
/// Lluvia de acido con temporizador. Ciclo: calma -> aviso -> lluvia -> calma...
/// Durante la lluvia hace dano periodico a la protagonista, salvo que este bajo un techo (refugio).
/// Ponlo en un objeto vacio de la escena.
/// </summary>
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

    [Header("Refugio (techos)")]
    [Tooltip("Capas que cuentan como techo (normalmente Ground).")]
    public LayerMask coverLayer;
    [Tooltip("Hasta que altura sobre la protagonista se busca un techo.")]
    public float coverCheckHeight = 8f;
    [Tooltip("Desde que altura sobre su centro se busca el techo (para no chocar con su propio collider).")]
    public float headOffset = 1f;

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

    private void DamageTick()
    {
        if (player == null || health == null) return;

        damageTimer -= Time.deltaTime;
        if (damageTimer > 0f) return;
        damageTimer = damageInterval;

        if (!IsExposed()) return;
        health.TakeDamage(damage, new Vector2(player.position.x, player.position.y + 5f), false);
    }

    /// <summary>True si no hay un techo encima de la protagonista (la lluvia la moja).</summary>
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
