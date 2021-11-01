using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackPlayerScript : MonoBehaviour
{
    private AudioManager m_AudioManager;
    private AudioSource m_AudioSource;
    private static SoundtrackPlayerScript m_Instance;

    public List<AudioClip> SoundtrackClips;
    public float Volume = 0.35f;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioManager = AudioManager.Instance;

        // Load up AudioClips
        foreach (AudioClip clip in SoundtrackClips)
        {
            AudioManager.Instance.AddSoundtrackClip(clip); // Addressing AudioManager.Instance but we have m_AudioManager... why?
        }

        m_AudioSource.clip = AudioManager.Instance.GetSoundtrackClip(0);
        m_AudioSource.volume = Volume;
        m_AudioSource.loop = true;
        m_AudioSource.Play();
    }
}