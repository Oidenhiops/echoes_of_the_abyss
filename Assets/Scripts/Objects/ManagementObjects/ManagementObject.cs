using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementObject : MonoBehaviour
{
    public ManagementInteract managementInteract;
    public GameObject meshObj;
    public Collider colliderForMap;
    public ObjectInfo objectInfo;
    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().UseObject(character, objectInfo, managementCharacterObjects);
    }
    public void DropObject(Character character ,ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().DropObject(character, objectInfo, managementCharacterObjects);
    }
    public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().InitializeObject(character, objectInfo, managementCharacterObjects);
    }
    public void GiveStatistics(){

    }
    public void RemoveStatistics(){

    }
    [System.Serializable]
    public class ObjectInfo
    {
        public ObjectsDataSO objectData;
        public int amount;
    }
    public interface IObject
    {
        public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
        public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
        public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
        public ManagementCharacterAnimations.TypeAnimation GetTypeObjAnimation();
    }
}
