using UnityEngine;

/// <summary>
/// Movimiento de la protagonista: caminar, saltar (con coyote time y salto en cola),
/// salto de altura variable, doble salto (poder) y empujon al recibir dano.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;
    public float groundAcceleration = 90f;
    public float airAcceleration = 60f;

    [Header("Salto")]
    public float jumpVelocity = 12f;
    [Tooltip("Gravedad extra al caer: hace la caida mas rapida y el salto mas 'firme'.")]
    public float fallGravityMultiplier = 2.2f;
    [Tooltip("Gravedad extra si sueltas el boton de salto antes de tiempo: salto corto.")]
    public float lowJumpGravityMultiplier = 2.6f;
    public float maxFallSpeed = 25f;
    [Tooltip("Segundos que aun puedes saltar despues de dejar el borde de una plataforma.")]
    public float coyoteTime = 0.1f;
    [Tooltip("Segundos que se recuerda un salto presionado un poco antes de tocar el suelo.")]
    public float jumpBufferTime = 0.12f;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.18f;
    public LayerMask groundLayer;

    [Header("Animator (opcional)")]
    public Animator animator;

    // Los poderes (PlayerPowerUps) cambian estos valores.
    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public int extraJumps = 0;   // saltos extra en el aire (doble salto = 1)

    public bool IsGrounded { get; private set; }
    public bool FacingRight { get; private set; }
    public bool CanControl { get { return inputEnabled && stunTimer <= 0f; } }

    private Rigidbody2D rb;
    private SafeAnimator anim;
    private float baseGravity;
    private float moveInput;
    private bool jumpHeld;
    private float coyoteCounter;
    private float bufferCounter;
    private int airJumpsUsed;
    private float stunTimer;
    private bool inputEnabled = true;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");

    private Vector2 Vel
    {
#if UNITY_6000_0_OR_NEWER
        get { return rb.linearVelocity; }
        set { rb.linearVelocity = value; }
#else
        get { return rb.velocity; }
        set { rb.velocity = value; }
#endif
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravity = rb.gravityScale;
        FacingRight = transform.localScale.x >= 0f;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        anim = new SafeAnimator(animator);
    }

    private void Update()
    {
        // Suelo: solo cuenta si no estamos subiendo (evita contarlo justo despues de saltar).
        bool touching = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
        IsGrounded = touching && Vel.y <= 0.1f;

        if (stunTimer > 0f) stunTimer -= Time.deltaTime;

        if (CanControl)
        {
            moveInput = GameInput.Horizontal();
            jumpHeld = GameInput.JumpHeld();
            if (GameInput.JumpPressed()) bufferCounter = jumpBufferTime;
        }
        else
        {
            moveInput = 0f;
            jumpHeld = false;
        }

        bufferCounter -= Time.deltaTime;

        if (IsGrounded)
        {
            coyoteCounter = coyoteTime;
            airJumpsUsed = 0;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (bufferCounter > 0f && CanControl)
        {
            if (coyoteCounter > 0f)
            {
                DoJump();
                coyoteCounter = 0f;
                bufferCounter = 0f;
            }
            else if (airJumpsUsed < extraJumps)
            {
                DoJump();
                airJumpsUsed++;
                bufferCounter = 0f;
            }
        }

        if (moveInput > 0.01f && !FacingRight) Flip();
        else if (moveInput < -0.01f && FacingRight) Flip();

        anim.SetFloat(SpeedHash, Mathf.Abs(Vel.x));
        anim.SetFloat(VelocityYHash, Vel.y);
        anim.SetBool(GroundedHash, IsGrounded);
    }

    private void FixedUpdate()
    {
        Vector2 v = Vel;

        if (stunTimer <= 0f)
        {
            float target = moveInput * moveSpeed * speedMultiplier;
            float accel = IsGrounded ? groundAcceleration : airAcceleration;
            v.x = Mathf.MoveTowards(v.x, target, accel * Time.fixedDeltaTime);
        }

        // Gravedad variable: cae mas rapido y el salto es mas corto si sueltas el boton.
        if (v.y < 0f) rb.gravityScale = baseGravity * fallGravityMultiplier;
        else if (v.y > 0f && !jumpHeld) rb.gravityScale = baseGravity * lowJumpGravityMultiplier;
        else rb.gravityScale = baseGravity;

        if (v.y < -maxFallSpeed) v.y = -maxFallSpeed;
        Vel = v;
    }

    private void DoJump()
    {
        Vel = new Vector2(Vel.x, jumpVelocity);
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 s = transform.localScale;
        s.x = -s.x;
        transform.localScale = s;
    }

    /// <summary>Empuja a la protagonista (por ejemplo al recibir dano) y le quita el control un momento.</summary>
    public void ApplyKnockback(Vector2 direction, float force, float stunSeconds)
    {
        stunTimer = stunSeconds;
        Vel = direction.normalized * force;
    }

    /// <summary>Activa o desactiva el control del jugador (pausa, muerte, escenas de cierre).</summary>
    public void SetControl(bool enabledControl)
    {
        inputEnabled = enabledControl;
        if (!enabledControl) moveInput = 0f;
    }

    public void Stop()
    {
        Vel = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
