using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public static bool isBossDead = false;

    [Header("Health Settings")]
    public int bossHP = 3;
    public GameObject deathEffect;
    public float hitCooldown = 1.0f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    [Header("Summon Settings")]
    public GameObject[] monsterPrefabs;
    public Transform[] summonPoints;
    public float minSummonTime = 3f;
    public float maxSummonTime = 7f;
    public float summonAnimDelay = 0.5f;
    public float summonRange = 15f; // 소환을 시작할 거리

    [Header("Look at Player")]
    public Transform playerTransform;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        isBossDead = false;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(SummonRoutine());
    }

    private void Update()
    {
        if (isDead) return;

        // ⭐ 플레이어 방향 바라보기 로직 정리
        if (playerTransform != null && !isInvincible)
        {
            if (playerTransform.position.x > transform.position.x)
            {
                // 플레이어가 오른쪽일 때 (이미지 방향에 따라 1 혹은 -1)
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // 플레이어가 왼쪽일 때
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || isInvincible) return;

        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerRb != null && playerRb.linearVelocity.y < -0.1f)
            {
                TakeDamage();
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            }
        }
    }

    void TakeDamage()
    {
        bossHP--;
        if (bossHP <= 0)
        {
            BossDeath();
        }
        else
        {
            if (anim != null) anim.SetTrigger("Hit");
            StartCoroutine(HitCooldownRoutine());
        }
    }

    IEnumerator HitCooldownRoutine()
    {
        isInvincible = true;
        float timer = 0;
        while (timer < hitCooldown)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    void BossDeath()
    {
        isDead = true;
        isBossDead = true;

        if (anim != null) anim.SetTrigger("Die");

        // 주변 모든 적 제거
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject minion in minions)
        {
            if (minion != gameObject) Destroy(minion);
        }

        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.7f);
    }

    IEnumerator SummonRoutine()
    {
        while (!isDead)
        {
            // ⭐ 거리 체크: 플레이어가 summonRange 안에 들어왔을 때만 소환 시작
            if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) < summonRange)
            {
                float waitTime = Random.Range(minSummonTime, maxSummonTime);
                yield return new WaitForSeconds(waitTime);

                if (!isDead && monsterPrefabs.Length > 0 && summonPoints.Length > 0)
                {
                    if (anim != null) anim.SetTrigger("Summon");
                    yield return new WaitForSeconds(summonAnimDelay);

                    int mIdx = Random.Range(0, monsterPrefabs.Length);
                    int pIdx = Random.Range(0, summonPoints.Length);
                    Instantiate(monsterPrefabs[mIdx], summonPoints[pIdx].position, Quaternion.identity);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}