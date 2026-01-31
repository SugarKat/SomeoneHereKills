using Unity.VisualScripting;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager instance;

    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    private void Awake()
    {
        instance = this;
    }

    public void ActivatePanel(bool isGameWon = false)
    {
        if(isGameWon)
        {
            gameWonPanel.SetActive(true);
        }
        else
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {

    }

    public void RestartGame()
    {

    }
}
