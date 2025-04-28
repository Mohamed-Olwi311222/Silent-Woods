using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
  [SerializeField] Image itemImage;
  private RectTransform iconRectTransform;
  [HideInInspector] public Transform parentAfterDrag;
  public Transform initalItemPosition;

  void Awake()
  {
    iconRectTransform = GetComponent<RectTransform>();
  }


  public void OnBeginDrag(PointerEventData eventData)
  {
    initalItemPosition = transform.parent;
    parentAfterDrag = initalItemPosition; //responsible for snapping back if the dragged item didnt hit a new slot transform
    transform.SetParent(transform.root);
    transform.SetAsLastSibling();
    itemImage.raycastTarget = false;
  }

  public void OnDrag(PointerEventData eventData)
  {
      InventoryManager.instance.isDragging = true;
      iconRectTransform.anchoredPosition += eventData.delta / InventoryManager.instance.mainCanvas.scaleFactor;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    InventoryManager.instance.isDragging = false;
    if (!IsInsideInventory(eventData.position))
    {
      string itemId = eventData.pointerDrag.gameObject.GetComponent<ItemIdentification>().uniqueId;
      InventoryManager.instance.Remove(itemId, eventData.pointerDrag.gameObject.transform);
      return;
    }
    transform.SetParent(parentAfterDrag);
    itemImage.raycastTarget = true;
  }

  private bool IsInsideInventory(Vector2 screenPosition)
  {
    return RectTransformUtility.RectangleContainsScreenPoint(InventoryManager.instance.inventoryPanel, screenPosition, null);
  }
}
