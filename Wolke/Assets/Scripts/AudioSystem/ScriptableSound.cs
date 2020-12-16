using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewAudioFile", menuName = "Create New Audio Object", order = 1)]
public class ScriptableSound : ScriptableObject
{
    public AudioClip[] AudioClips;
    [Tooltip("The audioMixer group this sound will be part of")] public AudioMixerGroup Output;
    public bool BypassEffects = false;
    public bool BypassListenerEffects = false;
    public bool BypassReverbZones = false;
    [Tooltip("0 = High, 256 = Low")] [Range(0, 256)] public int Priority = 100;
    [SerializeField] [Range(0, 1)] private float volume = 1f;
    [SerializeField] [Range(0, 1)] private float volumeRandom = 0f;

    /// <summary>
    /// Return the Volume that the clip will have
    /// </summary>
    public float Volume() => Mathf.Clamp(volume + Random.Range(-volumeRandom, volumeRandom), 0f, 1f);
    [SerializeField] [Range(-3, 3)] private float pitch = 0f;
    [SerializeField] [Range(0, 3)] private float randomPitch = 0f;
    /// <summary>
    /// Return the Pitch that the clip will have
    /// </summary>
    public float Pitch() => Mathf.Clamp(pitch + Random.Range(-randomPitch, randomPitch), -3f, 3f);
    [Tooltip("-1 = Left, 1 = Right")] [Range(-1, 1)] public float StereoPan = 0f;
    [Tooltip("0 = no 3D Sound, 1 = full 3D Sound")] [Range(0, 1)] public float SpacialBlend = 0.8f;
    [Range(0, 1.1f)] public float ReverbZoneMix = 0f;

    [Header("3D SoundSettings")]
    [Range(0, 5)] public float DopplerLevel = 1f;
    [Range(0, 360)] public float Spread = 90f;
    public AudioRolloffMode VolumeRollOff = AudioRolloffMode.Logarithmic;
    public float MinDistance = 1f;
    public float MaxDistance = 100f;

    /// <summary>
    /// Returns 1 soundclip from the list of clips
    /// </summary>
    public AudioClip Clip() => AudioClips[Mathf.FloorToInt(Random.Range(0, AudioClips.Length))];
}
