using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        
        public List<AudioData> sounds;
        
        [SerializeField] private AudioSource audioSource;
        
        private bool _isActive = true;
        
        public void SetActive(bool active)
        {
            _isActive = active;
        }

        public bool ActiveSelf() => _isActive;
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            EventManager.Instance.AddListener(EventNames.PlaySound, (audioTag, delay) => Play((AudioTag)audioTag, (float)delay));
            EventManager.Instance.AddListener(EventNames.PlaySound, (source, audioTag, delay) => Play((AudioSource)source, (AudioTag)audioTag, (float)delay));
        }
        
        private void Play(AudioTag audioTag, float delay = 0f)
        {
            StartCoroutine(PlayCoroutine(audioTag, delay));
        }
        
        private void Play(AudioSource source, AudioTag audioTag, float delay = 0f)
        {
            StartCoroutine(PlayCoroutine(source, audioTag, delay));
        }
        
        private IEnumerator PlayCoroutine(AudioTag audioTag, float delay = 0f)
        {
            if (!_isActive) yield break;

            yield return new WaitForSeconds(delay);
            
            var sound = sounds.Find(sound => sound.tag == audioTag);
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.Play();
        }
        
        private IEnumerator PlayCoroutine(AudioSource source, AudioTag audioTag, float delay = 0f)
        {
            if (!_isActive) yield break;

            yield return new WaitForSeconds(delay);
            
            var sound = sounds.Find(sound => sound.tag == audioTag);
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.Play();
        }
        
    }
    
}



[Serializable]
public struct AudioData
{
    public AudioTag tag;
    public AudioClip clip;
    public float volume;
}

public enum AudioTag
{
    BubblesFalling,
    BlastBubbles,
    CollectBubble,
    LevelSuccess,
    LevelFail,
}
