using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 4f;
    public float crouchSpeedMultiplier = 0.4f;

    [Header("Pulo")]
    public float jumpForce = 9f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Cuspe")]
    public Transform spitPoint;
    public GameObject spitPrefab;
    public float spitSpeed = 7f;
    public float spitCooldown = 0.6f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool isCrouching;
    private bool facingRight = true;
    private float nextSpitTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isCrouching = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator) animator.SetTrigger("Jump");
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextSpitTime)
        {
            Spit();
            nextSpitTime = Time.time + spitCooldown;
        }

        FlipSprite();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed * (isCrouching ? crouchSpeedMultiplier : 1f);
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    void Spit()
    {
        if (animator) animator.SetTrigger("Spit");

        if (spitPrefab != null && spitPoint != null)
        {
            GameObject cuspe = Instantiate(spitPrefab, spitPoint.position, Quaternion.identity);
            float dir = facingRight ? 1f : -1f;

            Rigidbody2D cuspeRb = cuspe.GetComponent<Rigidbody2D>();
            if (cuspeRb != null)
            {
                cuspeRb.linearVelocity = new Vector2(spitSpeed * dir, 0f);
            }

            SpriteRenderer cuspeSr = cuspe.GetComponent<SpriteRenderer>();
            if (cuspeSr != null && !facingRight)
            {
                cuspeSr.flipX = true;
            }
        }
    }

    void FlipSprite()
    {
        if (moveInput > 0 && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0 && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}