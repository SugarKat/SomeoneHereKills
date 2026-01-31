using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Scriptable Objects/GameRules")]
public class GameRules : ScriptableObject
{
    public float GameTime = 60f; // Time in seconds

    public int NpcCount = 20;

    [Range(0,1)]
    public float KillerDifficulty = .5f; // This for now, won't do much, but might use to determine the killers boldness
}
