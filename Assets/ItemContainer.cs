using System;

public interface IItemContainer
{
    // Add an item to the container
    bool AddItem(IItem item);
    
    // Remove an item from the container
    bool RemoveItem(IItem item);
    
    // Get an item at a specific index
    IItem GetItem(int index);
    
    // Clear all items
    void ClearItems();
    
    // Property to get the current item count
    int ItemCount { get; }
    
    // Property to check if the container is full
    bool IsFull { get; }
}

[Serializable]
public class ItemContainer : IItemContainer
{
    // Implementation of IItemContainer methods
    public bool AddItem(IItem item)
    {
        // Default implementation
        return false;
    }
    
    public bool RemoveItem(IItem item)
    {
        // Default implementation
        return false;
    }
    
    public IItem GetItem(int index)
    {
        // Default implementation
        return null;
    }
    
    public void ClearItems()
    {
        // Default implementation
    }
    
    public int ItemCount => 0;
    
    public bool IsFull => false;
}