using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannerTakeObjects : MonoBehaviour
{
    public Image backgroundObject;
    public Image spriteObject;
    public TMP_Text textObject;
    public ManagementCharacterObjects managementCharacterObjects;
    public GameObject objectForTake;
    public GameObject takeButton;
    public ManagementLanguage managementLanguage;
    public void TakeObject()
    {
        if (objectForTake.TryGetComponent<ManagementInteract>(out ManagementInteract managementInteract))
        {
            if (managementInteract.typeInteract == ManagementInteract.TypeInteract.Item)
            {
                managementCharacterObjects.TakeObject(objectForTake);
            }
            else if (managementInteract.typeInteract == ManagementInteract.TypeInteract.Object)
            {
                objectForTake.GetComponent<ManagementInteract.IObjectInteract>().Interact(managementCharacterObjects.character);
            }
        }
    }
}
