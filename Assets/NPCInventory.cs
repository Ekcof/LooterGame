using Mirror;
using System.Collections.Generic;
using UnityEngine;

public interface INPCInventory
{
    IEnumerable<IItemContainer> Containers { get; }
    
    // Add a method to get a specific container
    IItemContainer GetContainer(int index);
}

public class NPCInventory : NetworkBehaviour, INPCInventory
{
    [SerializeField] private int containerCount = 1;
    
    // List of ItemHolder components that will be used as containers
    private List<ItemHolder> _containers = new List<ItemHolder>();
    
    // Implement the Containers property to return the list of containers
    public IEnumerable<IItemContainer> Containers => _containers;
    
    // Initialize containers when the object starts
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeContainers();
    }
    
    // Initialize containers on the client as well
    public override void OnStartClient()
    {
        base.OnStartClient();
        InitializeContainers();
    }
    
    // Initialize the containers
    private void InitializeContainers()
    {
        // Clear existing containers
        _containers.Clear();
        
        // Create new containers
        for (int i = 0; i < containerCount; i++)
        {
            // Create a new GameObject for the container
            GameObject containerObject = new GameObject($"Container_{i}");
            containerObject.transform.SetParent(transform);
            
            // Add an ItemHolder component to the container
            ItemHolder itemHolder = containerObject.AddComponent<ItemHolder>();
            
            // Add the container to the list
            _containers.Add(itemHolder);
        }
    }
    
    // Get a specific container by index
    public IItemContainer GetContainer(int index)
    {
        if (index >= 0 && index < _containers.Count)
            return _containers[index];
            
        return null;
    }
}