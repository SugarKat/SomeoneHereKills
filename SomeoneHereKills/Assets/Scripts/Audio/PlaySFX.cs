using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;

    public bool playOnStart = false;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (clip == null) return;

        AudioManager.Instance.PlaySFXOneShot(clip, volume);
    }
}