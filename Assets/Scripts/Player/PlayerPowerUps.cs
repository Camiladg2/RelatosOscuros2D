using System;
using UnityEngine;

public enum PowerType
{
    None = 0,
    DobleSalto = 1,     // pocion naranja  - La Patasola
    EscudoAgua = 2,     // pocion cian     - La Llorona
    SaviaDeMonte = 3    // pocion lima     - La Madremonte
}

[Serializable]
public class PowerSettings
{
    public PowerType type;
    [Tooltip("Segundos que dura el poder.")]
    public float duration = 15f;
    [Tooltip("Objeto hijo con el aura o los destellos (se enciende mientras dura el poder).")]
    public GameObject aura;
}

/// <summary>
/// Maneja los 3 poderes de las pociones. Solo hay un poder activo a la vez:
/// si recoges otra pocion, reemplaza a la anterior.
/// </summary>
public class PlayerPowerUps : MonoBehaviour
{
    [Header("Configura los 3 poderes aqui")]
    public PowerSettings[] powers = new PowerSettings[]
    {
        new PowerSettings { type = PowerType.DobleSalto, duration = 15f },
        new PowerSettings { type = PowerType.EscudoAgua, duration = 15f },
        new PowerSettings { type = PowerType.SaviaDeMonte, duration = 15f }
    };

    [Header("Efectos")]
    public float doubleJumpSpeedBonus = 1.1f;
    public int saviaHealAmount = 1;

    [Header("Pruebas")]
    [Tooltip("Con esto activado, las teclas 1, 2 y 3 activan los poderes. Desactivalo al terminar.")]
    public bool testKeys = true;

    public PowerType Current { get; private set; }
    public float TimeLeft { get; private set; }
    public float CurrentDuration { get; private set; }
    public bool IsPlantImmune { get { return Current == PowerType.SaviaDeMonte; } }
    public bool ShieldUp { get; private set; }

    /// <summary>Se dispara al empezar un poder: (poder, duracion).</summary>
    public event Action<PowerType, float> OnPowerStarted;
    /// <summary>Se dispara al terminar un poder.</summary>
    public event Action<PowerType> OnPowerEnded;

    private PlayerController2D controller;
    private PlayerHealth health;
    private SafeAnimator anim;

    private static readonly int PowerUpHash = Animator.StringToHash("PowerUp");
    private static readonly int PowerTypeHash = Animator.StringToHash("PowerType");

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        health = GetComponent<PlayerHealth>();
        Animator a = GetComponentInChildren<Animator>();
        anim = new SafeAnimator(a);
        foreach (PowerSettings p in powers)
            if (p.aura != null) p.aura.SetActive(false);
    }

    private void Update()
    {
        if (testKeys)
        {
            if (GameInput.PowerKeyPressed(1)) Activate(PowerType.DobleSalto);
            if (GameInput.PowerKeyPressed(2)) Activate(PowerType.EscudoAgua);
            if (GameInput.PowerKeyPressed(3)) Activate(PowerType.SaviaDeMonte);
        }

        if (Current == PowerType.None) return;
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f) EndCurrent();
    }

    public void Activate(PowerType type)
    {
        if (type == PowerType.None) return;
        PowerSettings settings = Find(type);
        if (settings == null) return;

        if (Current != PowerType.None) EndCurrent();

        Current = type;
        CurrentDuration = settings.duration;
        TimeLeft = settings.duration;
        if (settings.aura != null) settings.aura.SetActive(true);

        switch (type)
        {
            case PowerType.DobleSalto:
                if (controller != null)
                {
                    controller.extraJumps = 1;
                    controller.speedMultiplier = doubleJumpSpeedBonus;
                }
                break;
            case PowerType.EscudoAgua:
                ShieldUp = true;
                break;
            case PowerType.SaviaDeMonte:
                if (health != null) health.Heal(saviaHealAmount);
                break;
        }

        anim.SetInt(PowerTypeHash, (int)type);
        anim.SetTrigger(PowerUpHash);
        if (OnPowerStarted != null) OnPowerStarted(type, settings.duration);
    }

    /// <summary>El escudo de agua absorbe un golpe. Devuelve true si lo absorbio.</summary>
    public bool TryAbsorbHit()
    {
        if (Current == PowerType.EscudoAgua && ShieldUp)
        {
            ShieldUp = false;
            EndCurrent();
            return true;
        }
        return false;
    }

    private void EndCurrent()
    {
        PowerType ended = Current;
        PowerSettings settings = Find(ended);
        if (settings != null && settings.aura != null) settings.aura.SetActive(false);

        if (controller != null)
        {
            controller.extraJumps = 0;
            controller.speedMultiplier = 1f;
        }
        ShieldUp = false;
        Current = PowerType.None;
        TimeLeft = 0f;
        if (OnPowerEnded != null) OnPowerEnded(ended);
    }

    private PowerSettings Find(PowerType type)
    {
        foreach (PowerSettings p in powers)
            if (p.type == type) return p;
        return null;
    }
}
