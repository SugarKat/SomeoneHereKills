using UnityEngine;

public class LevelAmbience : MonoBehaviour
{
    public AudioClip ambienceClip;
    [Range(0f, 1f)] public float volume = 0.6f;

    void Start()
    {
        if (ambienceClip != null)
            AudioManager.Instance.PlayAmbience(ambienceClip, volume);
    }
    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAmbience();
    }
}