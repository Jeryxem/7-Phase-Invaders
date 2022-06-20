using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [field: SerializeField] public string name { get; private set; }
    public AudioClip clip;

    [field: SerializeField, Range(0f,1f)] public float volume { get; private set; }
    [field: SerializeField, Range(.1f,3f)] public float pitch { get; private set; }
    [field: SerializeField] public bool loop { get; private set; }

    [HideInInspector]
    public AudioSource source;
}
