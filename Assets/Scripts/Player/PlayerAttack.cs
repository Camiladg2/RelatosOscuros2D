using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Vector2 fallbackOffset = new Vector2(0.6f, 0.6f);
    public float cooldown = 0.45f;
    public float fireDelay = 0.15f;
    public int baseDamage = 1;
    public int itemDamage = 2;

    [Header("Objeto que lleva ahora")]
    public WeaknessType heldItem = WeaknessType.None;
    public int itemCharges = 0;

    [Header("Tiempo del objeto")]
    [Tooltip("Cuando le quedan estos segundos, el objeto empieza a parpadear para avisar que se va a acabar.")]
    public float blinkWhenLeft = 3f;

    [Header("Icono del objeto (opcional)")]
    public SpriteRenderer heldIcon;

    /// <summary>Segundos que le quedan al objeto (0 si no tiene limite de tiempo).</summary>
    public float ItemTimeLeft { get; private set; }
    /// <summary>De 1 (recien recogido) a 0 (se acaba). Sirve para una barra en la UI.</summary>
    public float ItemTimeFraction { get { return itemHasTimer && itemDuration > 0f ? ItemTimeLeft / itemDuration : 1f; } }

    public event Action OnAttack;
    public event Action<WeaknessType, int> OnItemChanged;

    private PlayerController2D controller;
    private SafeAnimator anim;
    private float nextAttackTime;
    private Sprite heldSprite;
    private bool itemHasTimer;
    private float itemDuration;

    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        anim = new SafeAnimator(GetComponentInChildren<Animator>());
    }

    private void Start()
    {
        RefreshIcon();
    }

    private void Update()
    {
        UpdateItemTimer();

        if (!controller.CanControl) return;
        if (Time.time < nextAttackTime) return;
        if (!GameInput.AttackPressed()) return;

        nextAttackTime = Time.time + cooldown;
        anim.SetTrigger(AttackHash);
        if (OnAttack != null) OnAttack();
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        if (fireDelay > 0f) yield return new WaitForSeconds(fireDelay);
        Fire();
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;

        Vector3 position = firePoint != null ? firePoint.position : transform.TransformPoint(fallbackOffset);
        Vector2 direction = controller.FacingRight ? Vector2.right : Vector2.left;

        WeaknessType weakness = heldItem;
        Sprite spriteToThrow = null;
        int damage = baseDamage;
        if (heldItem != WeaknessType.None)
        {
            damage = itemDamage;
            spriteToThrow = heldSprite;
            UseCharge();
        }

        GameObject go = Instantiate(projectilePrefab, position, Quaternion.identity);
        AttackProjectile projectile = go.GetComponent<AttackProjectile>();
        if (projectile != null) projectile.Launch(direction, damage, weakness, spriteToThrow);
    }

    private void UpdateItemTimer()
    {
        if (heldItem == WeaknessType.None || !itemHasTimer) return;

        ItemTimeLeft -= Time.deltaTime;
        if (ItemTimeLeft <= 0f)
        {
            ClearItem();
            return;
        }

        // Parpadea cuando queda poco tiempo.
        if (heldIcon != null && heldSprite != null)
        {
            bool blinkOff = ItemTimeLeft < blinkWhenLeft && ((int)(ItemTimeLeft * 6f) % 2 == 0);
            heldIcon.enabled = !blinkOff;
        }
    }

    private void UseCharge()
    {
        itemCharges--;
        if (itemCharges <= 0)
        {
            ClearItem();
            return;
        }
        if (OnItemChanged != null) OnItemChanged(heldItem, itemCharges);
    }

    /// <summary>Quita el objeto de la mano (se acabo el tiempo o las cargas).</summary>
    public void ClearItem()
    {
        heldItem = WeaknessType.None;
        itemCharges = 0;
        heldSprite = null;
        itemHasTimer = false;
        itemDuration = 0f;
        ItemTimeLeft = 0f;
        RefreshIcon();
        if (OnItemChanged != null) OnItemChanged(heldItem, itemCharges);
    }

    /// <summary>La protagonista recoge un objeto. duration = segundos que dura (0 = sin limite de tiempo).</summary>
    public void GiveItem(WeaknessType type, int charges, Sprite icon, float duration)
    {
        heldItem = type;
        itemCharges = charges;
        heldSprite = icon;
        itemHasTimer = duration > 0f;
        itemDuration = duration;
        ItemTimeLeft = duration;
        RefreshIcon();
        if (OnItemChanged != null) OnItemChanged(heldItem, itemCharges);
    }

    public void GiveItem(WeaknessType type, int charges, Sprite icon)
    {
        GiveItem(type, charges, icon, 0f);
    }

    public void GiveItem(WeaknessType type, int charges)
    {
        GiveItem(type, charges, null, 0f);
    }

    private void RefreshIcon()
    {
        if (heldIcon == null) return;
        bool has = heldItem != WeaknessType.None;
        if (has && heldSprite != null) heldIcon.sprite = heldSprite;
        heldIcon.enabled = has && heldSprite != null;
    }
}