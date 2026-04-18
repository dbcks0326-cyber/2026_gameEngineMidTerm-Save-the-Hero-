using UnityEngine;
public class GhostEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color color;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
        color.a = 0.5f; // 처음엔 반투명하게
    }
    void Update()
    {
        color.a -= Time.deltaTime * 1f; // 쌩하고 사라지게 속도 조절
        sr.color = color;
        if (color.a <= 0) Destroy(gameObject); // 완전히 투명해지면 삭제
    }
}