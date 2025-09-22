using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Survival Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public string description;
    public Sprite icon;
    
    [Header("Stack Settings")]
    public int maxStackSize = 99;
    public bool isStackable = true;
    
    [Header("Item Type")]
    public ItemType itemType;
    
    [Header("Value")]
    public int baseValue = 1;
}

public enum ItemType
{
    Resource,
    Tool,
    Food,
    Equipment,
    Consumable
}
