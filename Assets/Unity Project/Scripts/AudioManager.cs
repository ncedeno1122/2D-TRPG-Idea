using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code 'closely imitated' from https://github.com/zenasquadratron/UnityGamePatternsExamples/blob/master/Assets/_Singleton/Scripts/SoundTrackPlayer.cs
// I've not tackled many audio systems before, so I took this. I want to implement those 'vertically-layered' tracks / adaptive music,
// so as my understanding advances I'll perhaps have to take that into account.
public class AudioManager
{
    private List<AudioClip> m_SoundtrackClips;

    private Dictionary<string, AudioClip> m_SFXClips;

    private static AudioManager m_Instance;

    public static AudioManager Instance
    {
        get
        {
            // Used for lazy instantiation
            if (m_Instance == null)
            {
                m_Instance = new AudioManager();
            }
            return m_Instance;
        }
    }

    public AudioManager()
    {
        // For lazy initialization :)
        m_SoundtrackClips = new List<AudioClip>();
        m_SFXClips = new Dictionary<string, AudioClip>();
    }

    // + + + + | Functions | + + + +

    public void PlaySoundtrackClip(int index, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_SoundtrackClips[index], position);
    }

    public AudioClip GetSoundtrackClip(int index)
    {
        return m_SoundtrackClips[index];
    }

    public void AddSoundtrackClip(AudioClip clip)
    {
        m_SoundtrackClips.Add(clip);
    }

    public void RemoveSoundtrackClip(AudioClip clip)
    {
        m_SoundtrackClips.Remove(clip);
    }

    // SFX

    public void PlaySFXByName(string name, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_SFXClips[name], position);
    }

    public void AddSFXByName(string name, AudioClip clip)
    {
        m_SFXClips.Add(name, clip);
    }

    public void RemoveSFXByName(string name)
    {
        m_SFXClips.Remove(name);
    }
}