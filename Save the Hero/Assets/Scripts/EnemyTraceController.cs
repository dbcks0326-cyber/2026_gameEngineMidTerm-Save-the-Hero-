using UnityEngine;

public class EnemyTraceController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float raycastDistance = .2f;
    public float traceDistance = 2f;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("플레이어 인식 성공! 이름: " + playerObj.name);
        }
        else
        {
            // 🚨 만약 이 로그가 뜬다면 태그 설정이 여전히 잘못된 것입니다.
            Debug.LogError("플레이어를 못 찾았습니다! 하이라키에서 Player 태그를 다시 확인하세요.");
        }
    }
    private void Update()
    {
        Vector2 direction = player.position - transform.position;

        if (direction.magnitude > traceDistance)
            return;

        Vector2 directionNormalized = direction.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach (RaycastHit2D rHit in hits)
        {

            if (rHit.collider != null && rHit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }
            else
            { 
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
              

        }
    }
}
