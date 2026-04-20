using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Settings")]
    public GameObject[] itemPrefabs;    // 소환할 아이템 프리팹들 (Speed, Jump, Invincible 등)
    public Transform[] spawnPoints;     // 아이템이 나타날 위치들 (빈 오브젝트들)

    [Header("Spawn Interval")]
    public float minSpawnTime = 5f;     // 최소 소환 간격 (초)
    public float maxSpawnTime = 12f;    // 최대 소환 간격 (초)

    [Header("Limit Settings")]
    public int maxItemCount = 3;        // 화면에 동시에 존재할 수 있는 최대 아이템 개수
    private int currentItemCount = 0;

    [Header("Detection Settings")]
    public Transform playerTransform; // 인스펙터에서 플레이어 할당
    public float detectionRange = 10f; // 이 거리 안에 들어오면 소환 시작

    void Start()
    {
        // 아이템 소환 루틴 시작
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // ⭐ 플레이어가 설정한 거리(detectionRange) 안에 있는지 확인
            if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) < detectionRange)
            {
                float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(waitTime);

                if (currentItemCount < maxItemCount && itemPrefabs.Length > 0 && spawnPoints.Length > 0)
                {
                    SpawnItem();
                }
            }
            else
            {
                // 멀리 있으면 대기
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void SpawnItem()
    {
        // 랜덤 아이템과 랜덤 위치 선정
        int itemIndex = Random.Range(0, itemPrefabs.Length);
        int pointIndex = Random.Range(0, spawnPoints.Length);

        // 아이템 생성
        GameObject newItem = Instantiate(itemPrefabs[itemIndex], spawnPoints[pointIndex].position, Quaternion.identity);

        // 아이템 개수 카운트 증가
        currentItemCount++;

        // 아이템이 먹히거나 파괴되었는지 감시해서 개수를 줄여주는 코루틴 시작
        StartCoroutine(TrackItemDestruction(newItem));
    }

    // 아이템이 사라지면(Player가 먹으면) 카운트를 줄여서 다음 소환이 가능하게 함
    IEnumerator TrackItemDestruction(GameObject item)
    {
        while (item != null)
        {
            yield return null;
        }
        currentItemCount--;
    }
}