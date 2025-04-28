using UnityEngine;
using System;
public class ItemPickUpController : MonoBehaviour
{
    public Item itemType;
    [SerializeField] public AudioClip[] pickupSounds;
    void Awake()
    {
        itemType = Instantiate(itemType);
    }
    public void Pickup()
    {
        itemType.id = Guid.NewGuid().ToString();
        if (InventoryManager.instance.Add(itemType))
        {
            AudioManager.instance.PlayRandomSoundFXClip(pickupSounds, transform, 1f, 0f, Sound.SoundType.Default, true);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full"); //TODO: display message for the player instead of console
        }
    }
}
