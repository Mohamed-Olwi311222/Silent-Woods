using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionController : MonoBehaviour
{
  [SerializeField] Camera playerCamera;
  [SerializeField] TextMeshProUGUI interactionText;
  [SerializeField] float interactionDistance = 5f;
  IInteractable currentTargetInteractable;
  [SerializeField] UnityEngine.UI.Image crosshair;

  public void FixedUpdate()
  {
    UpdateCurrentInteractable();
    UpdateInteractionTextAndCrosshair();
  }

  void UpdateCurrentInteractable()
  {
    var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
    Physics.Raycast(ray, out var hit, interactionDistance);
    currentTargetInteractable = hit.collider?.GetComponent<IInteractable>();
  }
  void UpdateInteractionTextAndCrosshair()
  {
    Color crosshairColor = crosshair.color;
    if (currentTargetInteractable == null)
    {
      interactionText.text = string.Empty;
      crosshairColor.a = 0.4f;
      crosshair.color = crosshairColor;
      return;
    }
    interactionText.text = currentTargetInteractable.interactMessage;
    crosshairColor.a = 1f;
    crosshair.color = crosshairColor;
  }
  public void Interact(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
    if (currentTargetInteractable != null)
    {
      currentTargetInteractable.Interact();
    }
    }
  }
}
