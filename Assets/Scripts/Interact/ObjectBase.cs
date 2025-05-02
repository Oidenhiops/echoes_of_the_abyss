using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    public ManagementInteract managementInteract;
    public GameObject meshObj;
    public Collider colliderForMap;
    public ObjectInfo objectInfo;
    public virtual void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Debug.LogError("Not implemented UseObject");
    }
    public virtual void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Debug.LogError("Not implemented DropObject");
    }
    public virtual void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Debug.LogError("Not implemented InitializeObject");
    }
    public virtual ManagementCharacterAnimations.TypeAnimation GetTypeObjAnimation()
    {
        Debug.LogError("Not implemented GetTypeObjAnimation");
        return ManagementCharacterAnimations.TypeAnimation.None;
    }
    [System.Serializable]   public class ObjectInfo
    {
        public ObjectsDataSO objectData;
        public int amount;
    }
}
