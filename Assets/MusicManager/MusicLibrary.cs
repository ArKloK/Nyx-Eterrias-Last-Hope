using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip clip;
}

public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] musicTracks;
    // Start is called before the first frame update
    public AudioClip GetClipFromName(string name)
    {
        foreach (MusicTrack musicTrack in musicTracks)
        {
            if (musicTrack.trackName == name)
            {
                return musicTrack.clip;
            }
        }
        Debug.LogWarning("Music track with name " + name + " not found!");
        return null;
    }
}
