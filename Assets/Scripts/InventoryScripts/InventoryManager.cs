using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    #region List of items and instantiated inventory slots
    public List<Item> slotItems = new List<Item>();
    private List<GameObject> inventorySlots = new List<GameObject>();
    #endregion /* List of items and instantiated inventory slots */

    #region Inventory Scene objects
    [Tooltip("The inventory slots parent that have the grid layout")]
    [SerializeField] Transform inventorySlotsParent;

    [Tooltip("The slot item object prefab")]
    [SerializeField] GameObject slotItemObjPrefab;

    [Tooltip("The empty item ScriptableObject")]
    [SerializeField] public Item emptySlotItem;

    [Tooltip("The Inventory slot prefab")]
    [SerializeField] public GameObject inventorySlot;

    [Tooltip("The player that can pickup and open the inventory")]
    [SerializeField] GameObject player;

    [Tooltip("The parent of the items that will be dropped by the player")]
    [SerializeField] Transform itemParent;

    [Tooltip("The inventory panel that should have grid layout on it")]
    [SerializeField] GameObject inventory;
    [Tooltip("The inventory Text image")]
    [SerializeField] GameObject inventoryText;
    #endregion /* Inventory Scene objects */

    #region Inventory Length
    [Tooltip("The maximum inventory items that the play can store")]
    [SerializeField] public int maximumInventoryLength = 6;
    private int currentInventoryLength = 0;
    #endregion /* Inventory Length */

    [Tooltip("The Canvas for knowing the scale of factor of the inventory")]
    [SerializeField] public Canvas mainCanvas;
    [Tooltip("The RectTransform of the current inventory panel")]
    [SerializeField] public RectTransform inventoryPanel;
    [HideInInspector] public bool inventoryState = false;
    [HideInInspector] public bool isDragging = false;

    [Tooltip("The message to display when the player pickup/remove an item")]
    [SerializeField] TMP_Text inventoryMessage;
    [Tooltip("The player crosshair to disable when opening the inventory")]
    [SerializeField] GameObject crosshair;

    #region script only variables
    private Transform slotTransform;
    private ItemIdentification itemIdentification;
    private InventorySlotIdentification inventorySlotIdentification;
    #endregion

    void Awake()
    {
        instance = this;
        for (int z = 0; z < maximumInventoryLength; z++)
        {
            CreateSlot(z);
        }
        inventoryText.SetActive(false);
        inventory.SetActive(false);
    }

    void CreateSlot(int _currentSlotIndex)
    {
        GameObject slot = Instantiate(inventorySlot, inventorySlotsParent);
        inventorySlotIdentification = slot.GetComponent<InventorySlotIdentification>();
        inventorySlotIdentification.slotIndex = _currentSlotIndex;
        inventorySlots.Add(slot);
        slotItems.Add(emptySlotItem);
    }

    public bool Add(Item item)
    {
        if (currentInventoryLength < maximumInventoryLength)
        {
            for (int idx = 0; idx < maximumInventoryLength; idx++)
            {
                slotTransform = inventorySlots[idx].transform;
                if (slotTransform.childCount == 0)
                {
                    //The slot is free
                    GameObject itemSlotObj = Instantiate(slotItemObjPrefab, slotTransform);
                    itemIdentification = itemSlotObj.GetComponent<ItemIdentification>();
                    PerformAdd(item, idx, itemSlotObj.transform, itemIdentification);
                    break;
                }
            }
            return true;
        }
        return false;
    }

    private void PerformAdd(Item item, int idx, Transform itemSlotObj, ItemIdentification itemIdentification)
    {
        Image slotIcon = itemSlotObj.Find("Icon").GetComponent<Image>();
        slotIcon.gameObject.SetActive(true);
        slotIcon.sprite = item.icon;
        itemIdentification.uniqueId = item.id;
        itemIdentification.uniqueIndex = idx;
        slotItems[idx] = item;
        currentInventoryLength++;
        // StartCoroutine(updateInventorytext(slotName.text + " has been added to your inventory", 3f));
    }

    public void Remove(string id, Transform draggedIconTransform)
    {
        GameObject currSlot;
        Transform slotTransform;
        Transform currSlotItemTransform;
        Image slotIcon;
        for (int idx = 0; idx < slotItems.Count; idx++)
        {
            currSlot = inventorySlots[idx];
            slotTransform = currSlot.transform;
            currSlotItemTransform = draggedIconTransform;
            slotIcon = currSlotItemTransform.Find("Icon").GetComponent<Image>();
            itemIdentification = currSlotItemTransform.GetComponent<ItemIdentification>();
            //drop the item in the slot if it is not 0(empty slot) and have an id
            if (id == itemIdentification.uniqueId && id == slotItems[idx].id)
            {
                PerformRemove(idx, slotIcon, itemIdentification);
                Destroy(currSlotItemTransform.gameObject);
                break;
            }
        }
    }

    private void PerformRemove(int idx, Image slotIcon, ItemIdentification itemIdentification)
    {
        // Drop the item in the game world
        DropItem(slotItems[idx]);
        // StartCoroutine(updateInventorytext(slotName.text + " has been removed from your inventory", 3f));
        slotItems[idx] = emptySlotItem;
        slotIcon.gameObject.SetActive(false);
        itemIdentification.uniqueId = "0";
        currentInventoryLength--;
    }

    private void DropItem(Item item)
    {
        Vector3 dropPosition = player.transform.position + player.transform.forward * 1.5f; // Drop slightly in front of the player
        GameObject droppedItem = Instantiate(item.prefab, dropPosition, Quaternion.identity, itemParent);
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(player.transform.forward * 2f, ForceMode.Impulse); // Give it a little push forward
        }
    }



    public void SwapItems(int initialSlotIndex, int targetSlotIndex, Transform draggableItemInitialPosition, Transform targetSlotItem)
    {
        //swap list items
        Item temp = slotItems[initialSlotIndex];
        slotItems[initialSlotIndex] = slotItems[targetSlotIndex];
        slotItems[targetSlotIndex] = temp;
        //swap items tranforms
        if (null != targetSlotItem)
        {
            targetSlotItem.SetParent(draggableItemInitialPosition);
        }
    }
    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (!isDragging)
        {
            inventoryState = !inventoryState;
            if (inventoryState)
            {
                inventoryText.SetActive(true);
                inventory.SetActive(true);
                if (null != crosshair)
                {
                    crosshair.SetActive(false);
                }
            }
            else
            {
                inventory.SetActive(false);
                inventoryText.SetActive(false);
                if (null != crosshair)
                {
                    crosshair.SetActive(true);
                }
            }
        }
    }
}
