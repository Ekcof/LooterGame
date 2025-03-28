using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemHolder : NetworkBehaviour, IItemContainer
{
    // SyncList to keep track of items across the network
    [SerializeField] private int maxItems = 10;
    
    // Use SyncList to automatically sync items across the network
    public readonly SyncList<ItemBase> items = new SyncList<ItemBase>();
    
    // Property to get the current item count
    public int ItemCount => items.Count;
    
    // Property to check if the holder is full
    public bool IsFull => items.Count >= maxItems;
    
    // Add an item to the holder
    public bool AddItem(IItem item)
    {
        if (IsFull || item == null)
            return false;
            
        if (item is ItemBase itemBase)
        {
            // Only the server can modify the SyncList
            if (isServer)
            {
                items.Add(itemBase);
                item.OnPut(this);
                return true;
            }
            else if (isClient)
            {
                // Request the server to add the item
                CmdAddItem(itemBase);
                return true;
            }
        }
        
        return false;
    }
    
    // Remove an item from the holder
    public bool RemoveItem(IItem item)
    {
        if (item == null)
            return false;
            
        if (item is ItemBase itemBase)
        {
            int index = items.IndexOf(itemBase);
            if (index != -1)
            {
                // Only the server can modify the SyncList
                if (isServer)
                {
                    items.RemoveAt(index);
                    item.OnTake(this);
                    return true;
                }
                else if (isClient)
                {
                    // Request the server to remove the item
                    CmdRemoveItem(index);
                    return true;
                }
            }
        }
        
        return false;
    }
    
    // Get an item at a specific index
    public IItem GetItem(int index)
    {
        if (index >= 0 && index < items.Count)
            return items[index];
            
        return null;
    }
    
    // Command to add an item (called from client, executed on server)
    [Command]
    private void CmdAddItem(ItemBase item)
    {
        if (!IsFull && item != null)
        {
            items.Add(item);
            item.OnPut(this);
        }
    }
    
    // Command to remove an item (called from client, executed on server)
    [Command]
    private void CmdRemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            ItemBase item = items[index];
            items.RemoveAt(index);
            item.OnTake(this);
        }
    }
    
    // Clear all items
    public void ClearItems()
    {
        if (isServer)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                ItemBase item = items[i];
                items.RemoveAt(i);
                item.OnTake(this);
            }
        }
        else if (isClient)
        {
            CmdClearItems();
        }
    }
    
    // Command to clear all items (called from client, executed on server)
    [Command]
    private void CmdClearItems()
    {
        ClearItems();
    }
}