using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Settings")]
    public Camera cam;
    public float parallaxEffectX; // 가로 이동 속도 비율 (0~1)
    public float parallaxEffectY; // 세로 이동 속도 비율 (추천: X의 절반 이하)

    private float length, startPosX, startPosY;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        startPosX = transform.position.x;
        startPosY = transform.position.y;

        // 가로 루프를 위한 이미지 길이 계산
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate() // 카메라 이동 후 실행되도록 LateUpdate 사용
    {
        // 1. 가로(X) 계산 및 무한 루프
        float distanceX = (cam.transform.position.x * parallaxEffectX);
        float tempX = (cam.transform.position.x * (1 - parallaxEffectX));

        // 2. 세로(Y) 계산
        float distanceY = (cam.transform.position.y * parallaxEffectY);

        // 3. 최종 위치 적용
        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);

        // 4. 가로 무한 루프 로직
        if (tempX > startPosX + length) startPosX += length;
        else if (tempX < startPosX - length) startPosX -= length;
    }
}