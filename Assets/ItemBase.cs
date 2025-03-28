using System;
using UnityEngine;

public enum ItemType
{
    None,
    Disposable,
    Ammo,
    Weapon,
    Armor,
    Container
}

public interface IItem
{
    string Id { get; }
    Sprite SpritePreview { get; }
    ItemType ItemType { get; }
    float Weight { get; }
    /// <summary>
    /// Total weight of stackable items
    /// </summary>
    float TotalWeight { get; }
    bool IsStackable { get; }
    /// <summary>
    /// if stackable it is possible to be => 1
    /// </summary>
    int Amount { get; }
    int AmountLimit { get; }
    bool IsBreakable { get; }
    /// <summary>
    /// acceptable if item is breakable
    /// </summary>
    float Durability { get; }
    float DurabilityLimit { get; }
    float Cost { get; }
    IItemContainer ParentContainer { get; }

    void OnTake(IItemContainer container);
    void OnPut(IItemContainer container);
    /// <summary>
    /// On destroy item
    /// </summary>
    void OnDestroyItem(IItemContainer container);
}

[Serializable]
public class ItemBase : IItem
{
    [SerializeField] protected string _id;
    [SerializeField] protected Sprite _spritePreview;
    [SerializeField] protected ItemType _itemType;
    [SerializeField] protected float _weight;
    [SerializeField] protected bool _isStackable;
    [SerializeField] protected int _amountLimit;
    [SerializeField] protected bool _isBreakable;
    [SerializeField] protected float _durabilityLimit;
    [SerializeField] protected float _cost;

    protected IItemContainer _parentContainer;
    protected float _durability;
    protected int _amount;

    public string Id => _id;
    public Sprite SpritePreview => _spritePreview;

    public ItemType ItemType => _itemType;

    public float Weight => _weight;

    public float TotalWeight => _weight * _amount;

    public bool IsStackable => _isStackable;

    public int AmountLimit => _amountLimit;

    public int Amount => _amount;
    public bool IsBreakable => _isBreakable;

    public float Durability => _durability;

    public float DurabilityLimit => _durabilityLimit;
    public float Cost => _cost;

    public IItemContainer ParentContainer => _parentContainer;
    public void OnDestroyItem(IItemContainer container)
    {
        throw new NotImplementedException();
    }

    public void OnPut(IItemContainer container)
    {
        throw new NotImplementedException();
    }

    public void OnTake(IItemContainer container)
    {
        throw new NotImplementedException();
    }
}
