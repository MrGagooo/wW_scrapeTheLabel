using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ScrapeTheLabel
{
    public class SoundManager : MicroMonoBehaviour
    {
        public static SoundManager _instance;
        public static SoundManager Instance { get { return _instance; } }

        

        //Private
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioSource audioSource2;
        [SerializeField]
        private AudioSource cursorAudioSource;

        [SerializeField]
        private AudioClip music96BPM;
        [SerializeField]
        private AudioClip music120BPM;
        [SerializeField]
        private AudioClip music160BPM;
        [SerializeField]
        private AudioClip music180BPM;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            
        }

        public void StartMusic()
        {
            switch (Macro.BPM)
            {
                case 96:
                    audioSource.clip = music96BPM;
                    break;
                case 120:
                    audioSource.clip = music120BPM;
                    break;
                case 160:
                    audioSource.clip = music160BPM;
                    break;
                case 180:
                    audioSource.clip = music180BPM;
                    break;
            }

            audioSource.Play();
        }

        public void PlayScrapingSound()
        {
            cursorAudioSource.Play();
        }

        public void StopScrapingSound()
        {
            cursorAudioSource.Stop();
        }

        public void PlayUntrapSound()
        {
            audioSource2.Play();
        }
    }
}
