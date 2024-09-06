using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private static AudioPlayer instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AudioPlayer Instance => instance;

    public void PlaySFX(string name)
    {
        AudioClip sfx = audioClips.Find(s => s.name == name);
        if (sfx != null)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}
