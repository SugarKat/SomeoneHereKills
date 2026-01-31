using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    List<Transform> interestPoints;

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        interestPoints = new List<Transform>();

        foreach (Transform child in transform)
        {
            interestPoints.Add(child);
        }
    }

    public Transform GetAnInterestPoint()
    {
        // Will maybe need to get some calculated interest point instead of random one
        if (interestPoints == null || interestPoints.Count == 0) return null;
        return interestPoints[Random.Range(0, interestPoints.Count)];
    }
}
