using UnityEngine;

public class PlayMusicPersistant : MonoBehaviour
{
    public AudioClip menuMusic;

    void Start()
    {
        AudioManager.Instance.PlayMusicPersistant(menuMusic);
    }

}