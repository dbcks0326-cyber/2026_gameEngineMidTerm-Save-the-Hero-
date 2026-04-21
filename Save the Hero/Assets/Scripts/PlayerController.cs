using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private Animator myAnimator;
    private int jumpCount = 0;
    private bool wasGrounded;
    private float doubleJumpTimer = 0f;
    // private bool isPreparingJump = false;
    // private float jumpTimer = 0f;
    [Header("Item: Jump Boost")]
    public float jumpBoostMultiplier = 1.5f; // 얼마나 높게 뛸지 (1.5배)
    public float jumpBoostTime = 5f;        // 지속 시간
    private float originalJumpForce;         // 원래 점프 힘 저장용

    [Header("Item: Speed Boost")]
    public float speedBoostMultiplier = 2f;
    public float speedBoostTime = 5f;
    private float originalMoveSpeed;

    [Header("Item: Invincibility")]
    public bool isInvincible = false;
    public float invincibilityTime = 3f;
    private SpriteRenderer spriteRenderer;

    [Header("Ghost Trail (잔상)")]
    public GameObject ghostPrefab;      // 유니티에서 프리팹 드래그 앤 드롭
    public float ghostDelay = 0.07f;
    private float ghostTimer;
    private bool isMakingGhost = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce; // ⭐ 원래 점프 힘을 미리 저장
    }

    private void Update()
    {
        // 1. 잔상 생성 로직
        if (isMakingGhost)
        {
            ghostTimer -= Time.deltaTime;
            if (ghostTimer <= 0)
            {
                if (ghostPrefab != null)
                {
                    GameObject currentGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
                    currentGhost.transform.localScale = transform.localScale;

                    SpriteRenderer ghostSR = currentGhost.GetComponent<SpriteRenderer>();
                    if (ghostSR != null)
                    {
                        if (ghostSR != null && spriteRenderer != null)
                        {
                            ghostSR.sprite = spriteRenderer.sprite;
                            // 잔상이 너무 밝아서 안 보일 수 있으니 색상잡기
                            ghostSR.color = new Color(1f, 1f, 1f, 0.6f);
                        }
                    }
                    Destroy(currentGhost, 0.3f);
                }
                ghostTimer = ghostDelay;
            }
        }

        // 2. 이동 및 바닥 체크
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

        // 3. 애니메이션 및 방향
        myAnimator.SetBool("move", Mathf.Abs(moveInput) > 0.1f);

        float yVelocity = rb.linearVelocity.y;
        if (!isGrounded)
        {
            if (yVelocity > 0.1f) { myAnimator.SetBool("Jump", true); myAnimator.SetBool("Fall", false); }
            else if (yVelocity < -0.1f && doubleJumpTimer <= 0f) { myAnimator.SetBool("Fall", true); myAnimator.SetBool("Jump", false); }
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
            StartCoroutine(InvincibilityRoutine());
            return;
        }

        if (collision.CompareTag("SpeedItem"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(SpeedBoostRoutine());
            return;
        }

        if (collision.CompareTag("JumpItem")) 
        {
            Destroy(collision.gameObject);
            StartCoroutine(JumpBoostRoutine());
            return;
        }
        if (collision.CompareTag("Respawn"))
        {
            if (isInvincible) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        // 1. 보스 약점(머리 위 오브젝트)을 밟았을 때
        if (collision.CompareTag("Boss"))
        {
            // 부모 오브젝트(Boss)에 있는 BossController를 가져옵니다.
            BossController boss = collision.GetComponentInParent<BossController>();

            if (boss != null && rb.linearVelocity.y < -0.1f)
            {
                boss.TakeDamage(); // 보스 대미지

                // 플레이어 반동
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            }
        }

        // 2. 일반 적이나 보스 몸체 트리거에 닿았을 때 (옆면 등)
        if (collision.CompareTag("Enemy"))
        {
            if (isInvincible) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            // 1. 현재 맵에 보스가 있는지 확인합니다.
            BossController boss = GameObject.FindObjectOfType<BossController>();

            if (boss != null)
            {
                // 2. 보스가 있다면? 죽었을 때만 통과!
                if (BossController.isBossDead)
                {
                    GoToNextLevel(collision);
                }
                else
                {
                    Debug.Log("보스를 아직 처치하지 않았습니다!");
                }
            }
            else
            {
                // 3. 보스가 없는 일반 스테이지라면? 그냥 통과!
                GoToNextLevel(collision);
            }
        }

    }


    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        yield return new WaitForSeconds(invincibilityTime);
        spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    private IEnumerator JumpBoostRoutine()
    {
        jumpForce = originalJumpForce * jumpBoostMultiplier;

        // 캐릭터 색상을 점프 물약 느낌으로 변경
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(0.5f, 1f, 0.5f, 1f);

        yield return new WaitForSeconds(jumpBoostTime);

        spriteRenderer.color = originalColor;
        jumpForce = originalJumpForce; // 원래대로 복구
    }
    private IEnumerator SpeedBoostRoutine()
    {
        moveSpeed = originalMoveSpeed * speedBoostMultiplier;
        isMakingGhost = true; //  잔상 켜기

        yield return new WaitForSeconds(speedBoostTime);

        isMakingGhost = false; //  잔상 끄기
        moveSpeed = originalMoveSpeed;
    }
    // 몬스터와 물리적으로 부딪혔을 때 (Is Trigger가 꺼져 있을 때)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
       {
           if (isInvincible) return; // 무적 상태면 살려줌

           // 씬 재시작 (죽음)
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
       }
  }

    void GoToNextLevel(Collider2D collision)
    {
        LevelObject lo = collision.GetComponent<LevelObject>();
        if (lo != null) lo.MoveToNextLevel();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}