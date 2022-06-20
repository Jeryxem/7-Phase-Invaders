using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [field: SerializeField] public Sound[] sounds { get; private set; }
    public static AudioManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    private void Start() {
        Play("Bg_Music");
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
