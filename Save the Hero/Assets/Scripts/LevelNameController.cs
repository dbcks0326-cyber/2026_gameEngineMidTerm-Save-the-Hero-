using UnityEngine;
using TMPro;
using System.Collections;

public class LevelNameController : MonoBehaviour
{
    private TextMeshProUGUI levelText;

    [Header("설정")]
    public string levelName = "마을 입구";
    public float fadeInTime = 1f;    // 나타나는 시간
    public float showTime = 2f;      // 유지되는 시간
    public float fadeOutTime = 1f;   // 사라지는 시간

    void Awake()
    {
        levelText = GetComponent<TextMeshProUGUI>();
        // 시작할 때는 글자를 투명하게 설정
        levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 0);
    }

    void Start()
    {
        // 씬이 시작되자마자 페이드 효과 시작!
        StartCoroutine(FadeLevelName());
    }

    IEnumerator FadeLevelName()
    {
        levelText.text = levelName;

        // 1. Fade In (나타나기)
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, alpha);
            yield return null;
        }

        // 2. Wait (잠시 대기)
        yield return new WaitForSeconds(showTime);

        // 3. Fade Out (사라지기)
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, alpha);
            yield return null;
        }

        // 4. 완료 후 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}