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

    // ─── Private Variables ──────────────────────────────────────────────
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isGrounded;
    bool isAlive = true;
    float horizontalInput;

    // Ladder support
    [Header("Climbing")]
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] LayerMask ladderLayer;

    bool isOnLadder;
    float originalGravity;

    // ─── Lifecycle ───────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (!isAlive) return;

        ReadInput();
        CheckGrounded();
        HandleJump();
        FlipSprite();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        Move();
        ApplyBetterJumpPhysics();
    }

    // ─── Input ───────────────────────────────────────────────────────────
    void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1
    }

    // ─── Movement ────────────────────────────────────────────────────────
    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Ground Check ────────────────────────────────────────────────────
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ─── Jump ────────────────────────────────────────────────────────────
    void HandleJump()
    {
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
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    // ─── Public API (called by other scripts) ───────────────────────────
    public void OnDeath()
    {
        isAlive = false;
        animator.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
            isOnLadder = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = false;
            rb.gravityScale = originalGravity;
        }
    }

    // ─── Climbing ────────────────────────────────────────────────────────
    void HandleClimb()
    {
        if (!isOnLadder) return;

        float verticalInput = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
        rb.gravityScale = (Mathf.Abs(verticalInput) > 0) ? 0f : originalGravity;
    }
}