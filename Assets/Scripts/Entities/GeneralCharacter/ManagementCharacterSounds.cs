using System;
using UnityEngine;

public class ManagementCharacterSounds : MonoBehaviour
{
    [NonSerialized] public CharacterSoundsSO soundsInfo;
    public AudioClip GetAudioClip(CharacterSoundsSO.TypeSound typeSound)
    {
        CharacterSoundsSO.SoundsInfo sounds = new CharacterSoundsSO.SoundsInfo();
        foreach (CharacterSoundsSO.SoundsInfo sound in soundsInfo.sounds)
        {
            if (sound.typeSound == typeSound)
            {
                sounds = sound;
                break;
            }
        }
        return sounds.clips[UnityEngine.Random.Range(0, sounds.clips.Length - 1)];
    }
}
