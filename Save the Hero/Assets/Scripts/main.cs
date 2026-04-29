using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{

    public GameObject helpPanel;

    public GameObject scorePanel;

    public void GameStart()
    {
        SceneManager.LoadScene("level 1");
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);

    }
    public void CloseHelp()
    {
        helpPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void mainScene()
    {
        SceneManager.LoadScene("Main");
    }
    public void bossScene()
    {
        SceneManager.LoadScene("level 5");
    }

    public void Openscore()
    {
        scorePanel.SetActive(true);

    }

    public void Closescore()
    {
        scorePanel.SetActive(false);

    }
}