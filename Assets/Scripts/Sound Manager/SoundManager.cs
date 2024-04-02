using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource; // 2D effects are for UI sounds

    void Awake()
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

    public void PlaySound3D(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
    public void PlaySound3D(string clipName, Vector3 position)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(clipName), position);
    }
    public void PlaySound2D(string clipName)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(clipName));
    }
}
