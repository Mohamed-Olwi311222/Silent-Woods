using UnityEngine;
using UnityEngine.Events;

public class InteractClass : MonoBehaviour , IInteractable
{ 
   public string interactMessage => objectInteractMessage;
   
  [SerializeField] string objectInteractMessage;

  [SerializeField] UnityEvent interactAction;

  
    public void Interact()
   {
     interactAction.Invoke();
   }
   
}
