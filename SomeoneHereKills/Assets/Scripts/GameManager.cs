using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameRules gameRules;

    bool targetKilled = false;

    private void Awake()
    {
        instance = this;

        if (gameRules == null) 
        {
            gameRules = new GameRules();
        }
    }

    public void KillEvent(BaseAI victim)
    {
        if (targetKilled)
        {
            return;
        }
        // Will need to handle events accordingly who got killed

        switch (victim.Role) 
        {
            case BaseAI.AgentRole.Bystander:
                // It's a bad thing we killed a bystander, we need to decide how to handle this situation
                break;

            case BaseAI.AgentRole.Target:
                // The killer got his target, so that means we lost
                victim.KillAgent();
                GameLost();
                break;
            
            case BaseAI.AgentRole.Killer:
                // We got the killer, so we won and gotta handle those conditions
                GameWon();
                break;
        }
    }

    void GameLost()
    {
        targetKilled = true;
        GameEndManager.instance.ActivatePanel(false);
        Debug.Log("The killer got the target, mission failed.");
    }

    void GameWon()
    {
        GameEndManager.instance.ActivatePanel(true);
        Debug.Log("We got the killer, target is saved.");
    }
}
