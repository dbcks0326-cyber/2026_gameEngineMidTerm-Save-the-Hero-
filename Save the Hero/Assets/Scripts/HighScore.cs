using UnityEditor.SceneManagement;
using UnityEngine;

public static class HighScore
{
    private const string KEY = "HighScore";

    public static int Load(int stage)
    {
        return PlayerPrefs.GetInt(KEY + "_" + stage, 0);
    }
    public static void TrySet(int stage, int newScore)
    {
        if (newScore <= Load(stage))
            return;

        PlayerPrefs.SetInt(KEY + '_' + stage, newScore);
        PlayerPrefs.Save();
    }

}
