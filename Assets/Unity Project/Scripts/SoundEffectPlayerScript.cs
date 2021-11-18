using System.Collections.Generic;
using UnityEngine;

namespace Unity_Project.Scripts
{
    public class SoundEffectPlayerScript : MonoBehaviour
    {
        private AudioManager m_AudioManager;
        private AudioSource m_AudioSource;
        private static SoundEffectPlayerScript m_Instance;

        public List<AudioClip> SFXClips;
        public float Volume = 0.75f;

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
            foreach (var clip in SFXClips)
            {
                AudioManager.Instance.AddSFXByName(clip.name, clip);
            }
        }
    }
}
