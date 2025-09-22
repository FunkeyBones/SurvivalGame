using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int maxSlots = 20;
    public List<InventoryItem> items = new List<InventoryItem>();
    
    [Header("Debug")]
    public bool debugMode = true;
    
    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        // Check if item already exists and can stack
        if (itemData.isStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem existingItem = items[i];
                if (existingItem.itemData == itemData)
                {
                    int availableSpace = itemData.maxStackSize - existingItem.quantity;
                    if (availableSpace > 0)
                    {
                        int amountToAdd = Mathf.Min(quantity, availableSpace);
                        existingItem.quantity += amountToAdd;
                        items[i] = existingItem;
                        quantity -= amountToAdd;
                        
                        if (debugMode)
                            Debug.Log($"Added {amountToAdd} {itemData.itemName} to existing stack. New quantity: {existingItem.quantity}");
                        
                        if (quantity <= 0) return true;
                    }
                }
            }
        }
        
        // Add new stacks if needed
        while (quantity > 0 && items.Count < maxSlots)
        {
            int amountForNewStack = Mathf.Min(quantity, itemData.maxStackSize);
            items.Add(new InventoryItem(itemData, amountForNewStack));
            quantity -= amountForNewStack;
            
            if (debugMode)
                Debug.Log($"Added new stack of {amountForNewStack} {itemData.itemName}");
        }
        
        // Return true if all items were added
        return quantity <= 0;
    }
    
    public int GetItemCount(ItemData itemData)
    {
        int totalCount = 0;
        foreach (var item in items)
        {
            if (item.itemData == itemData)
                totalCount += item.quantity;
        }
        return totalCount;
    }
    
    public bool RemoveItem(ItemData itemData, int quantity = 1)
    {
        int remainingToRemove = quantity;
        
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemData == itemData)
            {
                InventoryItem item = items[i];
                if (item.quantity <= remainingToRemove)
                {
                    remainingToRemove -= item.quantity;
                    items.RemoveAt(i);
                }
                else
                {
                    item.quantity -= remainingToRemove;
                    items[i] = item;
                    remainingToRemove = 0;
                }
                
                if (remainingToRemove <= 0) break;
            }
        }
        
        return remainingToRemove <= 0;
    }
    
    // Debug method to show inventory contents
    [ContextMenu("Show Inventory")]
    public void ShowInventory()
    {
        Debug.Log("=== INVENTORY CONTENTS ===");
        foreach (var item in items)
        {
            Debug.Log($"{item.itemData.itemName} x{item.quantity}");
        }
        Debug.Log($"Used slots: {items.Count}/{maxSlots}");
    }
}