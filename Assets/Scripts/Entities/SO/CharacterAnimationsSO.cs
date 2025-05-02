using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "ScriptableObjects/Character/CharacterAnimationsSO", order = 1)]
public class CharacterAnimationsSO : ScriptableObject
{
    public CharacterAnimationsInfo animationsInfo;
    [System.Serializable]
    public class CharacterAnimationsInfo
    {
        public float baseSpritePerTime = 0.1f;
        public float currentSpritePerTime = 0.1f;
        public int currentSpriteIndex = 0;
        public AnimationsInfo[] animations;
    }
    [System.Serializable]
    public class AnimationsInfo
    {
        public ManagementCharacterAnimations.TypeAnimation typeAnimation;
        public SpritesInfo[] spritesInfoDown;
        public SpritesInfo[] spritesInfoUp;
        public ManagementCharacterAnimations.TypeAnimationsEffects[] animationsEffects;
        public bool needAnimationEnd = false;
        public bool loop = false;
        public int frameToInstance = 0;
        public float speedSpritesPerTimeMultplier = 1;
        public GameObject instanceObj;
        public GameObject instance;
    }
    [System.Serializable] public class HandTransformInfo
    {
        public Vector3 pos;
        public Quaternion rotation;
    }
    [System.Serializable] public class SpritesInfo
    {
        public Sprite generalSprite;
        public Sprite handSprite;
        public Vector3 leftHandPosDL;
        public Vector3 leftHandPosDR;
        public Quaternion leftHandRotation;
        public Vector3 rightHandPosDL;
        public Vector3 rightHandPosDR;
        public Quaternion rightHandRotation;
    }
}
