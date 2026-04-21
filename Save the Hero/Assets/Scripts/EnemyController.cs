using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    public float moveForce = 3f;       // 점프할 때 앞으로 밀어주는 힘
    public float jumpForce = 5f;       // 위로 솟구치는 힘
    public float jumpInterval = 2f;    // 점프 간격

    private Rigidbody2D rb;
    private Animator anim;
    private bool isMovingRight = true;
    private float jumpTimer;
    private bool isGrounded;

    [Header("Ground Check")]
    public Transform groundCheck;      // 슬라임 발밑 위치
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;      // Ground 레이어 설정 필요

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpTimer = jumpInterval;

        // Z축 회전 고정 (넘어짐 방지)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // 1. 바닥 체크 (애니메이션 및 점프 가능 여부 판단)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        anim.SetBool("Ground", isGrounded); // 바닥에 있는지 애니메이터에 전달

        // 2. 점프 타이머
        if (isGrounded)
        {
            // 바닥에 있을 때만 타이머가 흐름
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                JumpMove();
                jumpTimer = jumpInterval;
            }
        }

        // 3. 방향 전환 (이미지 반전)
        FlipImage();
    }

    private void JumpMove()
    {
        // 1. 혹시라도 남아있을 수 있는 트리거를 싹 지워줍니다. (두 번 실행 방지 핵심!)
        anim.ResetTrigger("jump");

        // 2. 그 다음 깔끔하게 한 번만 실행!
        anim.SetTrigger("jump");

        // 방향 설정
        float direction = isMovingRight ? 1f : -1f;

        // 대각선 방향으로 힘을 줍니다.
        Vector2 jumpDirection = new Vector2(direction * moveForce, jumpForce);
        rb.AddForce(jumpDirection, ForceMode2D.Impulse);
    }

    private void FlipImage()
    {
        if (isMovingRight) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            // 1. 방향 전환
            isMovingRight = !isMovingRight;

            // 2. 방향을 틀자마자 살짝 앞으로 밀어주기 (연속 충돌 방지)
            float pushOffset = isMovingRight ? 0.1f : -0.1f;
            transform.position += new Vector3(pushOffset, 0, 0);

            // 3. 이미지 반전 즉시 적용
            FlipImage();
        }
    }
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            // 실제 바닥을 체크하는 범위를 빨간 원으로 그리기
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}