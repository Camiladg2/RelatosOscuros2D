using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float invulnerableTime = 1.5f;
    public float knockbackForce = 9f;
    public float stunTime = 0.25f;
    public float respawnInvulnerableTime = 1.5f;

    [Header("Visual (opcional)")]
    public SpriteRenderer spriteToBlink;
    public Animator animator;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvulnerable { get; private set; }
    public bool MurioPorCaida { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDamaged;
    public event Action OnDied;
    public event Action OnRespawned;

    private PlayerController2D controller;
    private PlayerPowerUps powerUps;
    private Rigidbody2D body;
    private SafeAnimator anim;
    private RigidbodyConstraints2D constraintsOriginales;

    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int ReviveHash = Animator.StringToHash("Revive");

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        powerUps = GetComponent<PlayerPowerUps>();
        body = GetComponent<Rigidbody2D>();
        if (body != null) constraintsOriginales = body.constraints;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteToBlink == null) spriteToBlink = GetComponentInChildren<SpriteRenderer>();
        anim = new SafeAnimator(animator);
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
    }

    public bool TakeDamage(int amount, Vector2 sourcePosition)
    {
        return TakeDamage(amount, sourcePosition, true);
    }

    public bool TakeDamage(int amount, Vector2 sourcePosition, bool applyKnockback)
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
            Die(false);
            return true;
        }

        if (applyKnockback)
        {
            float side = transform.position.x >= sourcePosition.x ? 1f : -1f;
            controller.ApplyKnockback(new Vector2(side, 0.8f), knockbackForce, stunTime);
        }
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

    public void Kill()
    {
        if (IsDead) return;
        CurrentHealth = 0;
        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
        Die(true);
    }

    public void Respawn(Vector3 position)
    {
        StopAllCoroutines();
        if (spriteToBlink != null) spriteToBlink.enabled = true;
        IsDead = false;
        MurioPorCaida = false;
        IsInvulnerable = false;
        CurrentHealth = maxHealth;

        if (body != null)
        {
            body.simulated = true;
            body.constraints = constraintsOriginales;
        }
        transform.position = position;
        if (body != null) body.position = position;
        Physics2D.SyncTransforms();

        controller.Stop();
        controller.SetControl(true);
        ResetTriggerSafe(HurtHash);
        ResetTriggerSafe(DieHash);
        anim.SetTrigger(ReviveHash);

        if (OnHealthChanged != null) OnHealthChanged(CurrentHealth, maxHealth);
        if (OnRespawned != null) OnRespawned();
        if (respawnInvulnerableTime > 0f) StartCoroutine(InvulnerabilityRoutine(respawnInvulnerableTime));
    }

    private void Die(bool congelar)
    {
        IsDead = true;
        MurioPorCaida = congelar;
        controller.Stop();
        controller.SetControl(false);

        if (body != null)
        {
            if (congelar)
                body.simulated = false;
            else
                body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        ResetTriggerSafe(ReviveHash);
        anim.SetTrigger(DieHash);
        if (OnDied != null) OnDied();
    }

    private void ResetTriggerSafe(int hash)
    {
        if (animator == null) return;
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.nameHash == hash)
            {
                animator.ResetTrigger(hash);
                return;
            }
        }
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