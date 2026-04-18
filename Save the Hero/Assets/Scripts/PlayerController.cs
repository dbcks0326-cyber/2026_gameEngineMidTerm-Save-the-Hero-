using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections; // 필수!

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private Animator myAnimator;
    private float jumpDelay = 0.25f;
    private float jumpTimer = 0f;
    private bool isPreparingJump = false;
    private int jumpCount = 0;
    public int maxJumpCount = 1;
    private bool wasGrounded;
    private float doubleJumpTimer = 0f;

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityTime = 3f;
    private SpriteRenderer spriteRenderer; // 👈 변수 선언 추가됨!

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // 여기서 가져옴
    }

    private void Update()
    {
        if (doubleJumpTimer > 0f) doubleJumpTimer -= Time.deltaTime;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
            myAnimator.ResetTrigger("DoubleJump");
            myAnimator.ResetTrigger("Jump");
        }
        wasGrounded = isGrounded;

        myAnimator.SetBool("move", Mathf.Abs(moveInput) > 0.1f);

        if (isPreparingJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isPreparingJump = false;
            }
        }

        float yVelocity = rb.linearVelocity.y;
        if (!isGrounded || isPreparingJump)
        {
            if (yVelocity > 0.1f) { myAnimator.SetBool("Jump", true); myAnimator.SetBool("Fall", false); }
            else if (yVelocity < -0.1f) { if (doubleJumpTimer <= 0f) { myAnimator.SetBool("Fall", true); myAnimator.SetBool("Jump", false); } }
        }
        else { myAnimator.SetBool("Jump", false); myAnimator.SetBool("Fall", false); }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>().x; }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed || jumpCount >= 2) return;
        jumpCount++;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (jumpCount == 2) { myAnimator.SetTrigger("DoubleJump"); doubleJumpTimer = 0.2f; }
        else myAnimator.SetTrigger("Jump");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(InvincibilityRoutine()); // 👈 이제 빨간 줄 안 뜸!
            return;
        }

        if (collision.CompareTag("Respawn") || collision.CompareTag("Enemy"))
        {
            if (isInvincible) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (collision.CompareTag("Finish"))
        {
            LevelObject lo = collision.GetComponent<LevelObject>();
            if (lo != null) lo.MoveToNextLevel();
            else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // ⭐ 이 코루틴 함수가 반드시 클래스 안에 있어야 합니다!
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(invincibilityTime);

        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        isInvincible = false;
    }
}