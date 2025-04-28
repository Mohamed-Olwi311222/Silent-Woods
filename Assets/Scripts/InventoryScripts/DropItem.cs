using UnityEngine;
using UnityEngine.EventSystems;

public class DropItem : MonoBehaviour, IDropHandler
{
    DragItem draggedItem;
    InventorySlotIdentification inventorySlotIdentification;


    void Awake()
    {
        inventorySlotIdentification = GetComponent<InventorySlotIdentification>();
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        draggedItem = droppedItem.GetComponent<DragItem>();
        draggedItem.parentAfterDrag = transform; // sets the dragged item transform to the current hit slot transform
        int targetSlotIndex, draggedItemTempIndex;
        SwapIndecies(droppedItem, out targetSlotIndex, out draggedItemTempIndex);
        HandleSwap(targetSlotIndex, draggedItemTempIndex);

    }

    private void SwapIndecies(GameObject droppedItem, out int targetSlotIndex, out int draggedItemTempIndex)
    {
        ItemIdentification droppedItemIdentification = droppedItem.GetComponent<ItemIdentification>();
        targetSlotIndex = inventorySlotIdentification.slotIndex;
        draggedItemTempIndex = droppedItemIdentification.uniqueIndex;
        droppedItemIdentification.uniqueIndex = targetSlotIndex;
    }

    private void HandleSwap(int targetSlotIndex, int draggedItemTempIndex)
    {
        if (transform.childCount != 0)
        {
            transform.GetChild(0).GetComponent<ItemIdentification>().uniqueIndex = draggedItemTempIndex;
            InventoryManager.instance.SwapItems(draggedItemTempIndex, targetSlotIndex, draggedItem.initalItemPosition, draggedItem.parentAfterDrag.GetChild(0));
        }
        else
        {
            InventoryManager.instance.SwapItems(draggedItemTempIndex, targetSlotIndex, draggedItem.initalItemPosition, null);
        }
    }
}