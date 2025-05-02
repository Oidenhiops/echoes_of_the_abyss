using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ManagementCharacterObjects : MonoBehaviour
{
    public Character character;
    public GameObject rightHandPos;
    public ObjectsInfo[] objects = new ObjectsInfo[6];
    [SerializeField] ObjectsPositionsInfo[] objectsPositionsInfo;
    public int objectSelectedPosition = 0;
    public void InitializeObjectsEvents()
    {
        character.characterInputs.characterActions.CharacterInputs.ChangeObject.performed += OnChangeObject;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject1.performed += OnChangeObject1;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject2.performed += OnChangeObject2;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject3.performed += OnChangeObject3;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject4.performed += OnChangeObject4;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject5.performed += OnChangeObject5;
        character.characterInputs.characterActions.CharacterInputs.ChangeObject6.performed += OnChangeObject6;
        character.characterInputs.characterActions.CharacterInputs.UseObject.performed += OnUseObject;
    }
    public void HandleObjects()
    {
        if (character.characterInfo.isActive)
        {

        }
    }
    public void OnChangeObject(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive)
        {
            ChangeCurrentObject(context.ReadValue<float>() > 0);
        }
    }
    public void OnChangeObject1(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(0);
        }
    }
    public void OnChangeObject2(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(1);
        }
    }
    public void OnChangeObject3(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(2);
        }
    }
    public void OnChangeObject4(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(3);
        }
    }
    public void OnChangeObject5(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(4);
        }
    }
    public void OnChangeObject6(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ChangeCurrentObject(5);
        }
    }
    public void OnUseObject(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && context.action.triggered)
        {
            ValidateUseItem();
        }
    }
    public void TakeObject(GameObject objectForTake)
    {
        bool pickUpItem = false;
        ObjectsInfo[] objectsFinded = FindObjects(objectForTake);
        if (objectsFinded.Length == 0)
        {
            objectsFinded = objects;
        }
        ManagementObject objectTaked = objectForTake.GetComponent<ManagementObject>();
        foreach (ObjectsInfo objectForValidate in objectsFinded)
        {
            if (objectForValidate.objectData != null && CanStackObject(objectForValidate, objectForTake) && objectTaked.objectInfo.amount > 0)
            {
                int amountToAdd = ValidateAmountObjectToAdd(objectForValidate, objectTaked);
                objectForValidate.amount += amountToAdd;
                character.characterInfo.characterScripts.managementCharacterHud.SendInformationMessage($"{GameData.Instance.GetDialog(17)} {amountToAdd} {GameData.Instance.GetDialog(objectTaked.managementInteract.IDText)}", Color.green);
                pickUpItem = true;
            }
        }
        if (objectTaked.objectInfo.amount > 0)
        {
            foreach (ObjectsInfo objectForAdd in objects)
            {
                if (objectForAdd.objectData == null && objectTaked.objectInfo.amount > 0)
                {
                    objectForAdd.objectData = objectTaked.objectInfo.objectData;
                    int amountToAdd = ValidateAmountObjectToAdd(objectForAdd, objectTaked);
                    objectForAdd.amount = amountToAdd;
                    character.characterInfo.characterScripts.managementCharacterHud.SendInformationMessage($"{GameData.Instance.GetDialog(17)} {amountToAdd} {GameData.Instance.GetDialog(objectTaked.managementInteract.IDText)}", Color.green);
                    pickUpItem = true;
                }
            }
        }
        if (objectTaked.objectInfo.amount > 0)
        {
            bool isFullInventory = true;
            foreach (ObjectsInfo validateFullInventory in objects)
            {
                if (validateFullInventory.objectData != null && validateFullInventory.objectData == objectTaked.objectInfo.objectData && validateFullInventory.amount < validateFullInventory.objectData.maxStack)
                {
                    isFullInventory = false;
                    break;
                }
            }
            if (isFullInventory)
            {
                character.characterInfo.characterScripts.managementCharacterHud.SendInformationMessage($"{GameData.Instance.GetDialog(18)}", Color.red);
            }
        }
        else
        {
            Destroy(objectTaked.gameObject);
        }
        if (pickUpItem)
        {
            character.characterInfo.PlayASound(CharacterSoundsSO.TypeSound.PickUp, false);
        }
        else
        {
            character.characterInfo.PlayASound(CharacterSoundsSO.TypeSound.NotPickup, false);
        }
        RefreshObjects();
    }
    public void ValidateUseItem()
    {
        if (character.characterInputs.characterActionsInfo.isSecondaryAction)
        {
            DropObject();
        }
        else
        {
            UseObject();
        }
    }
    public void InitializeObjects()
    {
        if (character.characterInfo.isPlayer) objects = (ObjectsInfo[])GameData.Instance.saveData.gameInfo.characterInfo.currentObjects.Clone();
        for (int i = 0; i < 6; i++){
            if (objects[i].objectData != null)
            {
                if (objects[i].amount == 0)
                {
                    objects[i].amount = 1;
                }
            }
        }
        ChangeCurrentObject(objectSelectedPosition);
        if (character.characterInfo.isPlayer) character.characterInfo.characterScripts.managementCharacterHud.RefreshObjects(objects);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].id = i;
            if (objects[i].objectData != null && objects[i].isUsingItem)
            {
                objects[i].objectData.objectInstance.GetComponent<ManagementObject>().InitializeObject(character, objects[i], this);
            }
        }
    }
    public void UseObject()
    {
        if (objects[objectSelectedPosition].objectData != null)
        {
            objects[objectSelectedPosition].objectData.objectInstance.GetComponent<ManagementObject>().UseObject(character, objects[objectSelectedPosition], this);
        }
    }
    public void DropObject()
    {
        if (objects[objectSelectedPosition].objectData != null)
        {
            objects[objectSelectedPosition].objectData.objectInstance.GetComponent<ManagementObject>().DropObject(character, objects[objectSelectedPosition], this);
        }
    }
    public void InstanceObjectInHand(GameObject objectInHand, bool isLeftHand)
    {
        if (isLeftHand)
        {

        }
        else
        {
            GameObject instance = Instantiate
            (
                objectInHand,
                character.characterInfo.characterScripts.characterAnimations.rightHand.transform.position,
                Quaternion.identity,
                rightHandPos.transform
            );
            instance.GetComponent<BoxCollider>().enabled = false;
            instance.GetComponent<Rigidbody>().isKinematic = true;
            instance.GetComponent<ManagementInteract>().canInteract = false;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void DestroyObjectInHand(bool isLeftHand)
    {
        if (isLeftHand)
        {

        }
        else
        {
            if (rightHandPos.transform.childCount > 0) Destroy(rightHandPos.transform.GetChild(0).gameObject);
        }
    }
    void ChangeCurrentObject(bool direction)
    {
        objectSelectedPosition += direction ? 1 : -1;
        if (objectSelectedPosition > objects.Length - 1)
        {
            objectSelectedPosition = 0;
        }
        else if (objectSelectedPosition < 0)
        {
            objectSelectedPosition = objects.Length - 1;
        }

        character.characterInfo.characterScripts.managementCharacterHud.ChangeObject(objectSelectedPosition);
    }
    void ChangeCurrentObject(int position)
    {
        objectSelectedPosition = position;
        character.characterInfo.characterScripts.managementCharacterHud.ChangeObject(objectSelectedPosition);
    }
    int ValidateAmountObjectToAdd(ObjectsInfo objectForIncreaseAmount, ManagementObject objectForDiscountAmount)
    {
        for (int i = 1; i <= objectForDiscountAmount.objectInfo.amount; i++)
        {
            if (objectForIncreaseAmount.amount + i == objectForIncreaseAmount.objectData.maxStack || objectForDiscountAmount.objectInfo.amount - i == 0)
            {
                objectForDiscountAmount.objectInfo.amount -= i;
                return i;
            }
        }
        return 0;
    }
    ObjectsInfo[] FindObjects(GameObject objectToFind)
    {
        List<ObjectsInfo> objectsFinded = new List<ObjectsInfo>();
        foreach (ObjectsInfo objectInfo in objects)
        {
            if (objectInfo.objectData == objectToFind.GetComponent<ManagementObject>().objectInfo.objectData)
            {
                objectsFinded.Add(objectInfo);
            }
        }
        return objectsFinded.ToArray();
    }
    bool CanStackObject(ObjectsInfo objectForValidate, GameObject objectForTake)
    {
        if (objectForValidate.objectData == objectForTake.GetComponent<ManagementObject>().objectInfo.objectData && objectForValidate.amount < objectForValidate.objectData.maxStack)
        {
            return true;
        }
        return false;
    }
    public void RefreshObjects()
    {
        foreach (ObjectsInfo objectsInfo in objects)
        {
            if (objectsInfo.objectData != null)
            {
                if (objectsInfo.amount <= 0)
                {
                    objectsInfo.objectData = null;
                }
            }
        }
        character.characterInfo.characterScripts.managementCharacterHud.RefreshObjects(objects);
    }
    public ObjectsPositionsInfo GetObjectsPositionsInfo(TypeObjectPosition typeObjectPosition)
    {
        foreach (var objectPositionInfo in objectsPositionsInfo)
        {
            if (objectPositionInfo.typeObjectPosition == typeObjectPosition)
            {
                return objectPositionInfo;
            }
        }
        return null;
    }
    [System.Serializable] public class ObjectsInfo
    {
        public ObjectsDataSO objectData;
        public int amount;
        public bool isUsingItem = false;
        public int id;
        public GameObject objectInstance;
    }
    [System.Serializable] public class ObjectsPositionsInfo{
        public TypeObjectPosition typeObjectPosition = TypeObjectPosition.None;
        public GameObject objectPosition;
    }
    public enum TypeObjectPosition
    {
        None = 0,
        Weapon = 1,
        Ligth = 2
    }
}