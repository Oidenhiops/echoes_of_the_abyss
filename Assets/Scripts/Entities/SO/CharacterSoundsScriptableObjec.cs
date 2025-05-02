using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterSounds", menuName = "ScriptableObjects/Character/CharacterSoundsSO", order = 1)]
public class CharacterSoundsSO : ScriptableObject
{
    public SoundsInfo[] sounds;
    [System.Serializable]
    public class SoundsInfo
    {
        public TypeSound typeSound;
        public AudioClip[] clips;
    }
    public enum TypeSound
    {
        None = 0,
        TakeDamage = 1,
        Slash = 2,
        PickUp = 3,
        NotPickup = 4,
        Die = 5
    }
}
