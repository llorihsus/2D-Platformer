using FMODUnity;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ─── Serialized Fields ──────────────────────────────────────────────
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpForce = 18f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] float fallMultiplier = 2.5f;   // faster fall
    [SerializeField] float lowJumpMultiplier = 2f;  // tap = small jump

    [Header("Climbing")]
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] LayerMask ladderLayer;

    // ─── Private Variables ──────────────────────────────────────────────
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isGrounded;
    bool isAlive = true;

    // Movement input
    float horizontalInput;
    float verticalInput;

    // Ladder state
    bool isOnLadder;   // inside ladder trigger
    bool isClimbing;   // actively climbing (pressing up/down)

    float originalGravity;

    // ─── Lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save default gravity so we can restore it after climbing
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!isAlive) return;

        ReadInput();
        CheckGrounded();
        HandleClimb();
        HandleJump();
        FlipSprite();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        Move();

        // Only apply extra gravity when NOT climbing
        if (!isClimbing)
        {
            ApplyBetterJumpPhysics();
        }
    }

    // ─── Input ───────────────────────────────────────────────────────────
    void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1
        verticalInput = Input.GetAxisRaw("Vertical");     // -1, 0, or 1 (for ladders)
    }

    // ─── Movement ────────────────────────────────────────────────────────
    void Move()
    {
        if (isClimbing)
        {
            // Climbing movement (vertical control replaces gravity)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * climbSpeed);
        }
        else
        {
            // Normal ground/air movement
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // ─── Ground Check ────────────────────────────────────────────────────
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ─── Jump ────────────────────────────────────────────────────────────
    void HandleJump()
    {
        // Prevent jumping while climbing
        if (isClimbing) return;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // ─── Better Jump Feel (Apex trick) ───────────────────────────────────
    void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            // Falling — apply extra gravity
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Released jump early — cut height
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // ─── Climbing ────────────────────────────────────────────────────────
    void HandleClimb()
    {
        // Not touching ladder → normal physics
        if (!isOnLadder)
        {
            isClimbing = false;
            rb.gravityScale = originalGravity;
            return;
        }

        // While on ladder, keep gravity off
        rb.gravityScale = 0f;

        // Move vertically using input
        rb.linearVelocity = new Vector2(0f, verticalInput * climbSpeed);

        // If no vertical input, stay attached and don't slide
        if (Mathf.Abs(verticalInput) < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }   

    // ─── Sprite Flip ────────────────────────────────────────────────────
    void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    // ─── Animator ────────────────────────────────────────────────────────
    void UpdateAnimator()
    {
        bool isRunning = Mathf.Abs(horizontalInput) > Mathf.Epsilon;

        // Disable run animation while climbing
        animator.SetBool("isRunning", isRunning && !isClimbing);

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        // Climbing animation control
        animator.SetBool("isClimbing", isOnLadder);
        animator.SetFloat("climbSpeed", 1f);
    }

    // ─── Trigger Detection (Ladders) ─────────────────────────────────────
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided object is on ladder layer
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = true;
            rb.gravityScale = 0f;       // Stop falling immediately
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = false;

            // Restore gravity when leaving ladder
            rb.gravityScale = originalGravity;
        }
    }

    // ─── Public API (called by other scripts) ───────────────────────────
    public void OnDeath()
    {
        isAlive = false;
        animator.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }
}