using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public int value;
    public ItemType itemType;
    public GameObject prefab;
    public enum ItemType
    {
        SanityItems,
        Keys,
        Empty,
    }
}
