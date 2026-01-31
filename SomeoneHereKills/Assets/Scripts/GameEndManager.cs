using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager instance;

    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    public GameObject killedInnocentPanel;
    public GameObject killedTargetPanel;

    private void Awake()
    {
        instance = this;
    }

    public void ActivatePanel(bool isGameWon = false, bool killedTarget = true)
    {
        if(isGameWon)
        {
            gameWonPanel.SetActive(true);
        }
        else
        {
            if(!killedTarget)
            {
                killedInnocentPanel.SetActive(true);
                killedTargetPanel.SetActive(false);
            }
            else
            {
                killedInnocentPanel.SetActive(false);
                killedTargetPanel.SetActive(true);
            }
            gameOverPanel.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
