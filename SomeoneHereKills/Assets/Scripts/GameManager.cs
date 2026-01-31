using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameRules gameRules;

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
        // Will need to handle events accordingly who got killed

        switch(victim.Role) 
        {
            case BaseAI.AgentRole.Bystander:
                // It's a bad thing we killed a bystander, we need to decide how to handle this situation
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

    void GameLost()
    {

    }

    void GameWon()
    {

    }
}
