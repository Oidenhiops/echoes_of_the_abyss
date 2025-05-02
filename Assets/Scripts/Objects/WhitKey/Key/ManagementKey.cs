using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ManagementKey : MonoBehaviour, ManagementObject.IObject
{
    public TypeKey typeKey;
    public LayerMask layerMask;
    public AudioClip unlockClip;
    public AudioClip noUnlockClip;
    public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Vector3 positionsSpawn = character.transform.position + new Vector3(character.characterInfo.characterScripts.managementCharacterModelDirection.movementDirectionAnimation.x, 0.5f, character.characterInfo.characterScripts.managementCharacterModelDirection.movementDirectionAnimation.y);
        GameObject objectInstance = Instantiate(objectInfo.objectData.objectInstance, positionsSpawn, Quaternion.identity, character.gameObject.transform);
        Vector3 directionForce = (character.transform.position - objectInstance.transform.position).normalized;
        objectInstance.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        objectInstance.GetComponent<ManagementInteract>().canInteract = true;
        objectInstance.GetComponent<ManagementObject>().objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.characterScripts.managementCharacterObjects.RefreshObjects();
        character.characterInfo.PlayASound(character.characterInfo.characterScripts.managementCharacterSounds.GetAudioClip(CharacterSoundsSO.TypeSound.PickUp), true);
    }
    public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects){}
    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects){}
    public ManagementCharacterAnimations.TypeAnimation GetTypeObjAnimation()
    {
        return ManagementCharacterAnimations.TypeAnimation.None;
    }
    public enum TypeKey
    {
        None = 0,
        General = 1,
        Special = 2
    }
}
