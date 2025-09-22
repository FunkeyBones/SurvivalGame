[System.Serializable]
public struct InventoryItem
{
    public ItemData itemData;
    public int quantity;
    
    public InventoryItem(ItemData data, int qty)
    {
        itemData = data;
        quantity = qty;
    }
}