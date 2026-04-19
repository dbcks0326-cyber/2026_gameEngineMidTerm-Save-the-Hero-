using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleManager : MonoBehaviour
{

    public GameObject helpPanel;

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
  
}