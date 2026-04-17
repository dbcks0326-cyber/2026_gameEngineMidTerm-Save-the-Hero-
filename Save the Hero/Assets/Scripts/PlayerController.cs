using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }



    private void Update()
    {

        if (doubleJumpTimer > 0f)
        {
            doubleJumpTimer -= Time.deltaTime;
        }
        // 이동
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);



        // 🔥 착지 "순간"만 감지
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;

            // 🔥 남아있는 트리거 제거 (핵심)
            myAnimator.ResetTrigger("DoubleJump");
            myAnimator.ResetTrigger("Jump");
        }

        wasGrounded = isGrounded;

        // 이동 애니
        myAnimator.SetBool("move", Mathf.Abs(moveInput) > 0.1f);

        // ⭐ 점프 딜레이 (한 번만!)
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

        // ⭐ 점프 / 낙하 애니 (항상 실행)
        float yVelocity = rb.linearVelocity.y;

        if (!isGrounded || isPreparingJump)
        {
            if (yVelocity > 0.1f)
            {
                myAnimator.SetBool("Jump", true);
                myAnimator.SetBool("Fall", false);
            }
            else if (yVelocity < -0.1f)
            {
                // 🔥 더블점프 직후에는 Fall 막기
                if (doubleJumpTimer <= 0f)
                {
                    myAnimator.SetBool("Fall", true);
                    myAnimator.SetBool("Jump", false);
                }
            }
            else if (isPreparingJump)
            {
                myAnimator.SetBool("Jump", true);
            }
        }
        else
        {
            myAnimator.SetBool("Jump", false);
            myAnimator.SetBool("Fall", false);
        }

        // 방향
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (isGrounded)
        {
            myAnimator.SetBool("Fall", false);
            myAnimator.SetBool("Jump", false);
        }
    }


    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;

    }


    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (jumpCount >= 2) return;

        int currentJump = jumpCount;
        jumpCount++;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (currentJump == 1)
        {
            myAnimator.SetTrigger("DoubleJump");
            doubleJumpTimer = 0.2f;
        }
        else
        {
            myAnimator.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Respawn"))
        { 

        }
        if (collision.CompareTag("Finish"))
        {

        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        collision.GetComponent<LevelObject>().MoveToNextLevel();

        }
}