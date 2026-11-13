using System.Text;
using UnityEngine;

/// <summary>
/// Diagnostico del Jugador. Por defecto escribe en la Console (no en la pantalla del juego).
/// Ponlo en el Jugador. Al terminar, quitalo o desmarca las casillas.
/// </summary>
public class PlayerDebugHUD : MonoBehaviour
{
    [Header("Donde mostrar la informacion")]
    [Tooltip("Dibuja el panel en la ventana Game.")]
    public bool showOnScreen = false;
    [Tooltip("Escribe mensajes en la Console.")]
    public bool logToConsole = true;

    [Header("Que escribir en la Console")]
    [Tooltip("Un resumen con [OK] y [X] al dar Play.")]
    public bool logAtStart = true;
    [Tooltip("Avisos cuando empieza o termina un poder, cambia la vida o ataca.")]
    public bool logEvents = true;
    [Tooltip("Un aviso cada vez que la protagonista toca o deja el suelo.")]
    public bool logGroundChanges = false;

    private PlayerController2D controller;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerPowerUps powerUps;
    private PlayerHealth health;
    private PlayerAttack attack;
    private GUIStyle style;
    private bool lastGrounded;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        powerUps = GetComponent<PlayerPowerUps>();
        health = GetComponent<PlayerHealth>();
        attack = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        if (powerUps != null)
        {
            powerUps.OnPowerStarted += OnPowerStarted;
            powerUps.OnPowerEnded += OnPowerEnded;
        }
        if (health != null)
        {
            health.OnHealthChanged += OnHealthChanged;
            health.OnDied += OnDied;
        }
        if (attack != null)
        {
            attack.OnAttack += OnAttack;
            attack.OnItemChanged += OnItemChanged;
        }
    }

    private void OnDisable()
    {
        if (powerUps != null)
        {
            powerUps.OnPowerStarted -= OnPowerStarted;
            powerUps.OnPowerEnded -= OnPowerEnded;
        }
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDied -= OnDied;
        }
        if (attack != null)
        {
            attack.OnAttack -= OnAttack;
            attack.OnItemChanged -= OnItemChanged;
        }
    }

    private void Start()
    {
        if (logToConsole && logAtStart) Debug.Log("[Diagnostico]\n" + BuildReport());
        if (controller != null) lastGrounded = controller.IsGrounded;
    }

    private void Update()
    {
        if (!logToConsole || !logGroundChanges || controller == null) return;
        if (controller.IsGrounded != lastGrounded)
        {
            lastGrounded = controller.IsGrounded;
            Debug.Log("[Suelo] " + (lastGrounded ? "Toco el suelo" : "Dejo el suelo"));
        }
    }

    // ---- avisos en la Console ----
    private void OnPowerStarted(PowerType type, float duration)
    {
        if (logToConsole && logEvents) Debug.Log("[Poder] Empezo: " + type + " (" + duration.ToString("0") + " s)");
    }

    private void OnPowerEnded(PowerType type)
    {
        if (logToConsole && logEvents) Debug.Log("[Poder] Termino: " + type);
    }

    private void OnHealthChanged(int current, int max)
    {
        if (logToConsole && logEvents) Debug.Log("[Vida] " + current + "/" + max);
    }

    private void OnDied()
    {
        if (logToConsole && logEvents) Debug.Log("[Vida] La protagonista murio");
    }

    private void OnAttack()
    {
        if (logToConsole && logEvents) Debug.Log("[Ataque] Lanzo la chispa");
    }

    private void OnItemChanged(WeaknessType item, int charges)
    {
        if (logToConsole && logEvents) Debug.Log("[Objeto] " + item + " (cargas: " + charges + ")");
    }

    /// <summary>Para pedir el resumen cuando quieras: tres puntitos del componente > Imprimir diagnostico ahora.</summary>
    [ContextMenu("Imprimir diagnostico ahora")]
    private void PrintNow()
    {
        Debug.Log("[Diagnostico]\n" + BuildReport());
    }

    // ---- panel en pantalla (solo si lo activas) ----
    private void OnGUI()
    {
        if (!showOnScreen) return;
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 15;
            style.normal.textColor = Color.white;
        }
        GUI.Label(new Rect(10, 10, 760, 900), BuildReport(), style);
    }

    private static string Ok(bool good) { return good ? "[OK] " : "[X]  "; }

    private bool HasParam(string name)
    {
        if (anim == null || anim.runtimeAnimatorController == null) return false;
        foreach (AnimatorControllerParameter p in anim.parameters)
            if (p.name == name) return true;
        return false;
    }

    private string BuildReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== MOVIMIENTO ===");
        sb.AppendLine(Ok(controller != null) + "Script PlayerController2D en el Jugador");
        sb.AppendLine(Ok(rb != null) + "Rigidbody2D en el Jugador");
        if (controller != null)
        {
            sb.AppendLine(Ok(controller.groundCheck != null) + "Ground Check asignado");
            sb.AppendLine(Ok(controller.groundLayer.value != 0) + "Ground Layer elegido");
            sb.AppendLine("En el suelo: " + (controller.IsGrounded ? "SI" : "NO"));
            sb.AppendLine("Control activo: " + (controller.CanControl ? "SI" : "NO"));

            if (controller.groundCheck != null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(controller.groundCheck.position, controller.groundCheckRadius, ~0);
                int shown = 0;
                if (hits != null)
                {
                    foreach (Collider2D h in hits)
                    {
                        if (h.transform == transform || h.transform.IsChildOf(transform)) continue;
                        sb.AppendLine("El sensor toca: " + h.name + " (capa " + LayerMask.LayerToName(h.gameObject.layer) + ")");
                        shown++;
                    }
                }
                if (shown == 0) sb.AppendLine("El sensor no toca nada");
            }
        }
        sb.AppendLine("Capa del Jugador: " + LayerMask.LayerToName(gameObject.layer));

        sb.AppendLine();
        sb.AppendLine("=== ANIMACION ===");
        sb.AppendLine(Ok(anim != null) + "Animator encontrado" + (anim != null ? " en: " + anim.gameObject.name : ""));
        if (anim != null)
        {
            sb.AppendLine(Ok(anim.runtimeAnimatorController != null) + "Animator Controller asignado");
            sb.AppendLine(Ok(anim.enabled) + "Animator activado");
            int count = GetComponentsInChildren<Animator>(true).Length;
            sb.AppendLine(Ok(count == 1) + "Animators en el Jugador: " + count + " (debe ser 1)");
            sb.AppendLine(Ok(HasParam("Speed")) + "Parametro Speed");
            sb.AppendLine(Ok(HasParam("Grounded")) + "Parametro Grounded");
            sb.AppendLine(Ok(HasParam("VelocityY")) + "Parametro VelocityY");
        }

        sb.AppendLine();
        sb.AppendLine("=== VIDA, PODERES Y ATAQUE ===");
        if (health != null) sb.AppendLine("Vida: " + health.CurrentHealth + "/" + health.maxHealth);
        else sb.AppendLine("[X]  Falta el script PlayerHealth");
        if (powerUps != null) sb.AppendLine("Poder activo: " + powerUps.Current + "   Escudo de agua listo: " + (powerUps.ShieldUp ? "SI" : "NO"));
        else sb.AppendLine("[X]  Falta el script PlayerPowerUps");
        if (attack != null)
        {
            sb.AppendLine(Ok(attack.projectilePrefab != null) + "Prefab de la chispa asignado");
            sb.AppendLine("Objeto de debilidad: " + attack.heldItem + " (cargas: " + attack.itemCharges + ")");
        }
        else sb.AppendLine("[X]  Falta el script PlayerAttack");
        return sb.ToString();
    }
}