using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : NetworkBehaviour, IItemContainer
{
    [SerializeField] private int _capacity = 10;

    [SyncVar]
    private float _totalWeight = 0f;

    // List to store items
    private readonly SyncList<ItemBase> _items = new SyncList<ItemBase>();

    public IReadOnlyList<ItemBase> Items => _items;
    public int Capacity => _capacity;
    public float TotalWeight => _totalWeight;
    public int ItemCount => _items.Count;
    public bool IsFull => _items.Count >= _capacity;

    #region Unity Callbacks

    protected override void OnValidate()
    {
        base.OnValidate();
        // Ensure capacity is always positive
        _capacity = Mathf.Max(1, _capacity);
    }

    void Awake()
    {
        // Initialize if needed
    }

    #endregion

    #region Server Methods

    [Server]
    public bool AddItem(ItemBase item)
    {
        if (item == null || IsFull)
            return false;

        _items.Add(item);
        _totalWeight += item.TotalWeight;
        item.OnPut(this);

        return true;
    }

    [Server]
    public bool RemoveItem(ItemBase item)
    {
        if (item == null || !_items.Contains(item))
            return false;

        _totalWeight -= item.TotalWeight;
        _items.Remove(item);
        item.OnTake(this);

        return true;
    }

    [Server]
    public ItemBase GetItem(int index)
    {
        if (index < 0 || index >= _items.Count)
            return null;

        return _items[index];
    }

    [Server]
    public void Clear()
    {
        foreach (var item in _items)
        {
            item.OnTake(this);
        }

        _items.Clear();
        _totalWeight = 0f;
    }

    #endregion

    #region Client Methods

    // Add client-specific methods if needed

    #endregion

    #region Network Callbacks

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Server-specific initialization
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Client-specific initialization
    }

    #endregion
}