using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour, IActivatable
{
    [SerializeField] protected ScriptableSound sound;
    [SerializeField] protected SoundLocation soundLocation;
    [SerializeField] protected Space soundPositionSpace = Space.World;
    [SerializeField] [Tooltip("The world-space/Local position where the sound is played if Sound Location = Position")] protected Vector3 soundPosition;
    [SerializeField] [Tooltip("Only needed if the Sound Location is = Object")] protected GameObject soundObject;

    public void Activate()
    {
        switch (soundLocation)
        {
            case SoundLocation.Here:
                AudioManager.Instance.PlaySound(sound, gameObject);
                break;
            case SoundLocation.Position:

                switch (soundPositionSpace)
                {
                    case Space.Self:
                        AudioManager.Instance.PlaySound(sound, soundPosition + transform.position);
                        break;
                    case Space.World:
                        AudioManager.Instance.PlaySound(sound, soundPosition);
                        break;
                    default:
                        break;
                }
                break;
            case SoundLocation.Object:
                if (soundObject != null)
                {
                    AudioManager.Instance.PlaySound(sound, soundObject);
                }
                else
                {
                    Debug.Log($"There is no sound object selected for", this);
                }
                break;
            case SoundLocation.Camera:
                AudioManager.Instance.PlaySound(sound);
                break;
        }
    }

    public enum SoundLocation
    {
        [Tooltip("Plays on this object")] Here,
        [Tooltip("Plays at the Sound Position (World Space)")] Position,
        [Tooltip("Plays at another gameobject")] Object,
        [Tooltip("Plays at the currently used camera")] Camera
    }
}
