using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ManagerModule<AudioManager>
{
    private List<AudioSource> audioSources = new List<AudioSource>();
    private List<AudioSource> audioObjects = new List<AudioSource>();

    private void Update()
    {
        for (int i = audioSources.Count - 1; i >= 0; i--)
        {
            if (audioSources[i] == null)
            {
                audioSources.RemoveAt(i);
                continue;
            }

            if (!audioSources[i].isPlaying)
            {
                Destroy(audioSources[i]);
                audioSources.RemoveAt(i);
            }
        }

        for (int i = audioObjects.Count - 1; i >= 0; i--)
        {
            if (audioObjects[i] == null)
            {
                audioObjects.RemoveAt(i);
                continue;
            }

            if (!audioObjects[i].isPlaying)
            {
                Destroy(audioObjects[i].gameObject);
                audioObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Plays a new Sound
    /// </summary>
    /// <param name="audioClip">the audioClip to be played</param>
    /// <param name="position">the position where the clip will be played</param>
    public void PlaySound(ScriptableSound sound, Vector3 position)
    {
        AudioSource temp = Instantiate(new GameObject(), position, Quaternion.identity, transform).AddComponent<AudioSource>();
        temp.clip = sound.Clip();
        ApplySettingsToAudioSource(sound, temp);
        temp.Play();
        audioObjects.Add(temp);
    }

    /// <summary>
    /// Plays a new Sound at the camera
    /// </summary>
    /// <param name="audioClip">the audioClip to be played</param>
    public void PlaySound(ScriptableSound sound)
    {
        AudioSource temp = Camera.current.gameObject.AddComponent<AudioSource>();
        temp.clip = sound.Clip();
        ApplySettingsToAudioSource(sound, temp);
        temp.Play();
        audioSources.Add(temp);
    }

    /// <summary>
    /// Plays a new Sound at a given gameObject
    /// </summary>
    /// <param name="audioClip">the audioClip to be played</param>
    /// <param name="gameObject">the gameObject</param>
    public void PlaySound(ScriptableSound sound, GameObject gameObject)
    {
        AudioSource temp = gameObject.AddComponent<AudioSource>();
        temp.clip = sound.Clip();
        ApplySettingsToAudioSource(sound, temp);
        temp.Play();
        audioSources.Add(temp);
    }

    /// <summary>
    /// Applies audioSettings to an audioSource
    /// </summary>
    /// <param name="audioSetting">the settings to apply</param>
    /// <param name="source">the source to apply to</param>
    private void ApplySettingsToAudioSource(ScriptableSound sound, AudioSource source)
    {
        source.outputAudioMixerGroup = sound.Output;
        source.bypassEffects = sound.BypassEffects;
        source.bypassListenerEffects = sound.BypassListenerEffects;
        source.bypassReverbZones = sound.BypassReverbZones;
        source.priority = sound.Priority;
        source.volume = sound.Volume();
        source.pitch = sound.Pitch();
        source.panStereo = sound.StereoPan;
        source.spatialBlend = sound.SpacialBlend;
        source.reverbZoneMix = sound.ReverbZoneMix;
        source.dopplerLevel = sound.DopplerLevel;
        source.spread = sound.Spread;
        source.rolloffMode = sound.VolumeRollOff;
        source.minDistance = sound.MinDistance;
        source.maxDistance = sound.MaxDistance;
    }
}
