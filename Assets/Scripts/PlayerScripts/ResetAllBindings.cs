using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    public void ResetBindings()
    {
        if (inputActions)
        {
            foreach (InputActionMap map in inputActions.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
        } 
    }
}
