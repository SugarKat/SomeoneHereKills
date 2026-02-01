using NUnit.Framework;
using UnityEngine;

public class GamePopulator : MonoBehaviour
{
    public GameObject killerPrefab;
    public GameObject targetPrefab;
    public GameObject bystanderPrefab;

    public Transform[] killerSpawnPoints;
    public Transform[] targetSpawnPoints;
    public Transform[] bystanderSpawnPoints;

    private void Start()
    {
        Transform position = killerSpawnPoints[Random.Range(0, killerSpawnPoints.Length)];
        Instantiate(killerPrefab, position.position, Quaternion.identity);

        position = targetSpawnPoints[Random.Range(0, targetSpawnPoints.Length)];
        Instantiate(targetPrefab, position.position, Quaternion.identity);

        PopulateLevel(GameManager.instance.gameRules.NpcCount);
    }

    void PopulateLevel(int count)
    {
        if(bystanderSpawnPoints.Length <= 0)
        {
            Debug.LogWarning("No spawn points set, need to set up the points before populating");
            return;
        }

        int j = 0;

        for(int i = 0; i < count; i++)
        {
            if(j >= bystanderSpawnPoints.Length)
                j = 0;

            Vector3 rnd = Random.insideUnitCircle;
            Vector3 pos = new Vector3(bystanderSpawnPoints[j].position.x + rnd.x, 0, bystanderSpawnPoints[j].position.z + rnd.z);

            Instantiate(bystanderPrefab, pos, Quaternion.identity);

            j++;
        }
    }
}
