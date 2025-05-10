using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class ManagementCharacterInteract : MonoBehaviour
{
    public Character character;
    public GameObject[] _currentInteracts;
    public event Action<GameObject[]> OnInteractsChanged;
    public GameObject[] currentInteracts
    {
        get => _currentInteracts;
        set
        {
            if (!_currentInteracts.SequenceEqual(value))
            {
                _currentInteracts = value;
                OnInteractsChanged?.Invoke(_currentInteracts);
            }
        }
    }
    Vector3 offset = new Vector3(0, 0.5f, 0);
    Vector3 size = new Vector3(1.5f, 1.5f, 1.5f);
    public LayerMask layerMask;
    public int currentObjectForTakePosition = 0;
    float delayChangeObjectForTake = 0;
    public GameObject currentObject;
    public void InitializeInteractsEvents()
    {
        OnInteractsChanged += HandleInteracts;
        character.characterInputs.characterActions.CharacterInputs.ChangeInteractable.performed += OnChangeObjectForTake;
        character.characterInputs.characterActions.CharacterInputs.Interact.performed += OnInteract;
    }
    public void Interact()
    {
        if (character.characterInfo.isActive)
        {
            if (character.characterInfo.isPlayer)
            {
                currentInteracts = CheckInteracts();
                if (currentInteracts.Length > 0)
                {
                    if (delayChangeObjectForTake > 0)
                    {
                        delayChangeObjectForTake -= Time.deltaTime;
                    }
                }
            }
        }
    }
    void OnChangeObjectForTake(InputAction.CallbackContext context)
    {
        if (delayChangeObjectForTake <= 0 && currentInteracts.Length > 0)
        {
            if (context.ReadValue<Vector2>().y > 0)
            {
                ChangeObjectForTake(true);
            }
            else if (context.ReadValue<Vector2>().y < 0)
            {
                ChangeObjectForTake(false);
            }

        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        if (character.characterInfo.isActive && currentObject && context.action.triggered)
        {
            HanldeInteracObject();
        }
    }
    void HanldeInteracObject()
    {
        currentObject.GetComponent<ManagementInteract>().Interact(character);
    }
    void HandleInteracts(GameObject[] objects)
    {
        currentObjectForTakePosition = 0;
        if (objects.Length > 0)
        {
            character.characterInfo.characterScripts.managementCharacterHud.characterUi.objectsUi.bannerTakeObjects.SetActive(true);
            character.characterInfo.characterScripts.managementCharacterHud.RefreshObjectsForTake(objects);
            currentObject = objects[currentObjectForTakePosition];
            character.characterInfo.characterScripts.managementCharacterHud.RefreshCurrentObjectForTake(objects, currentObject, currentObjectForTakePosition);
        }
        else
        {
            character.characterInfo.characterScripts.managementCharacterHud.characterUi.objectsUi.bannerTakeObjects.SetActive(false);
        }
    }
    GameObject[] CheckInteracts()
    {
        RaycastHit[] hits = Physics.BoxCastAll
        (
            transform.position + offset,
            size / 2,
            Vector3.up,
            Quaternion.identity,
            0,
            layerMask
        );
        List<GameObject> objectsChecked = new List<GameObject>();
        foreach (var objectForChecked in hits)
        {
            if (objectForChecked.collider.GetComponent<ManagementInteract>().canInteract)
            {
                objectsChecked.Add(objectForChecked.collider.gameObject);
            }
        }
        return objectsChecked.ToArray();
    }
    void ChangeObjectForTake(bool direction)
    {
        delayChangeObjectForTake = 0.25f;
        if (!direction)
        {
            currentObjectForTakePosition++;
            if (currentObjectForTakePosition > currentInteracts.Length - 1) currentObjectForTakePosition = 0;
        }
        else
        {
            currentObjectForTakePosition--;
            if (currentObjectForTakePosition < 0) currentObjectForTakePosition = currentInteracts.Length - 1;
        }
        currentObject = currentInteracts[currentObjectForTakePosition];
        character.characterInfo.characterScripts.managementCharacterHud.RefreshCurrentObjectForTake(currentInteracts, currentObject, currentObjectForTakePosition);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = currentInteracts.Length > 0 ? Color.cyan : Color.blue;
        Gizmos.DrawWireCube
        (
            transform.position + offset,
            size
        );
    }
}
