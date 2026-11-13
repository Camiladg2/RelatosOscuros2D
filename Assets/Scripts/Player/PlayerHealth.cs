using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Vida de la protagonista: recibir dano con empujon, tiempo de invulnerabilidad con parpadeo,
/// morir y reaparecer. La interfaz (UI) se conecta con los eventos de abajo.
/// </summary>
[RequireComponent(typeof(PlayerController2D))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float invulnerableTime = 1.5f;
    public float knockbackForce = 9f;
    public float stunTime = 0.25f;

    [Header("Visual (opcional)")]
    public SpriteRenderer spriteToBlink;
    public Animator animator;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvulnerable { get; private set; }

    /// <summary>(vida actual, vida maxima). La UI se suscribe aqui.</summary>
    public event Action<int, int> OnHealthChanged;
    public event Action OnDamaged;
    /// <summary>Se dispara al morir: aqui se activa el Game Over.</summary>
    public event Action OnDied;

    private PlayerController2D controller;
    private PlayerPowerUps powerUps;
    private SafeAnimator anim;

    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int ReviveHash = Animator.StringToHash("Revive");

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        powerUps = GetComponent<PlayerPowerUps>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteToBlink == null) spriteToBlink = GetComponentInChildren<SpriteRenderer>();
        anim = new SafeAnimator(animator);
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
    }

    /// <summary>Quita vida. Devuelve true si realmente hizo dano.</summary>
    public bool TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (IsDead || IsInvulnerable) return false;

        if (powerUps != null && powerUps.TryAbsorbHit())
        {
            StartCoroutine(InvulnerabilityRoutine(0.5f));
            return false;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
        if (OnDamaged != null) OnDamaged();

        if (CurrentHealth <= 0)
        {
            Die();
            return true;
        }

        float side = transform.position.x >= sourcePosition.x ? 1f : -1f;
        controller.ApplyKnockback(new Vector2(side, 0.8f), knockbackForce, stunTime);
        anim.SetTrigger(HurtHash);
        StartCoroutine(InvulnerabilityRoutine(invulnerableTime));
        return true;
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
    }

    /// <summary>Mata al jugador de inmediato (por ejemplo, caer al vacio).</summary>
    public void Kill()
    {
        if (IsDead) return;
        CurrentHealth = 0;
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
        Die();
    }

    /// <summary>Devuelve al jugador a la vida en una posicion (reaparecer).</summary>
    public void Respawn(Vector3 position)
    {
        StopAllCoroutines();
        if (spriteToBlink != null) spriteToBlink.enabled = true;
        IsDead = false;
        IsInvulnerable = false;
        CurrentHealth = maxHealth;
        transform.position = position;
        controller.Stop();
        controller.SetControl(true);
        anim.SetTrigger(ReviveHash);
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        IsDead = true;
        controller.Stop();
        controller.SetControl(false);
        anim.SetTrigger(DieHash);
        if (OnDied != null) OnDied();
    }

    private IEnumerator InvulnerabilityRoutine(float seconds)
    {
        IsInvulnerable = true;
        float end = Time.time + seconds;
        while (Time.time < end)
        {
            if (spriteToBlink != null) spriteToBlink.enabled = !spriteToBlink.enabled;
            yield return new WaitForSeconds(0.08f);
        }
        if (spriteToBlink != null) spriteToBlink.enabled = true;
        IsInvulnerable = false;
    }
}
