using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameRules gameRules;

    public GameObject pausePanel;

    bool targetKilled = false;
    bool isGameStarted = false;
    float timer;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        instance = this;

        if (gameRules == null) 
        {
            gameRules = new GameRules();
        }

        timer = gameRules.GameTime;
    }

    private void Update()
    {
        if (!isGameStarted)
            return;

        if(timer <= 0)
        {
            Debug.Log("game has ended");
            GameLost();
            return;
        }

        timer -= Time.deltaTime;
    }

    public void KillEvent(BaseAI victim)
    {
        if (victim == null || !victim.IsAgentAlive)
            return;

        victim.KillAgent();
        // Will need to handle events accordingly who got killed

        switch (victim.Role) 
        {
            case BaseAI.AgentRole.Bystander:
                Debug.Log("You eliminated an innocent!");
                // It's a bad thing we killed a bystander, we need to decide how to handle this situation
                GameLost(true);
                break;

            case BaseAI.AgentRole.Target:
                // The killer got his target, so that means we lost
                GameLost();
                break;
            
            case BaseAI.AgentRole.Killer:
                // We got the killer, so we won and gotta handle those conditions
                GameWon();
                break;
        }
    }

    void GameLost(bool killedInnocent = false)
    {
        if(killedInnocent)
        {
            GameEndManager.instance.ActivatePanel(false, false);

            return;
        }
        targetKilled = true;
        GameEndManager.instance.ActivatePanel(false);
        Debug.Log("The killer got the target, mission failed.");
    }

    void GameWon()
    {
        GameEndManager.instance.ActivatePanel(true);
        Debug.Log("We got the killer, target is saved.");
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        IsPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
        // Show pause UI here
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);
        IsPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
        // Hide pause UI here
    }

}
