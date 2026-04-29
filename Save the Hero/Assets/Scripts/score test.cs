using TMPro;
using UnityEngine;

public class scoretest : MonoBehaviour
{

    public TextMeshProUGUI level1;
    public TextMeshProUGUI level2;
    public TextMeshProUGUI level3;
    public TextMeshProUGUI level4;
    public TextMeshProUGUI level5;



    void Start()
    {
        level1.text = "level 1 : " + HighScore.Load(1).ToString();
        level2.text = "level 2 : " + HighScore.Load(2).ToString();
        level3.text = "level 3 : " + HighScore.Load(3).ToString();
        level4.text = "level 4 : " + HighScore.Load(4).ToString();
        level5.text = "level 5 : " + HighScore.Load(5).ToString();
        Debug.Log(HighScore.Load(1));
    }

}
