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
    int SlotSize { get; }
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
    ItemBase GetCopy(int count = 1);
    void SetAmount(int count);
}

[Serializable]
public abstract class ItemBase : IItem
{
    [SerializeField] protected string _id;
    [SerializeField] protected Sprite _spritePreview;
    [SerializeField] protected ItemType _itemType;
    [SerializeField] protected float _weight;
    [SerializeField] protected bool _isStackable;
    [SerializeField, ShowIf("_isStackable", true)] protected int _amountLimit;
    [SerializeField] protected bool _isBreakable;
    [SerializeField] protected float _durabilityLimit;
    [SerializeField] protected float _cost;
    [Min(1), SerializeField] protected int _slotSize;
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
	public int SlotSize => _slotSize;

	public IItemContainer ParentContainer => _parentContainer;


	public abstract ItemBase GetCopy(int count = 1);
    public abstract void OnDestroyItem(IItemContainer container);
    public abstract void OnPut(IItemContainer container);
    public abstract void OnTake(IItemContainer container);

	public virtual void SetAmount(int count)
	{
		if(_amount == 0)
        {
            _amount = count;
		}
	}
}


[Serializable]
public abstract class ItemBase<T> : ItemBase where T : ItemBase
{
    public sealed override ItemBase GetCopy(int count = 1)
    {
        var serialized = JsonUtility.ToJson(this);
        var copy = JsonUtility.FromJson<T>(serialized);
        copy.SetAmount(count);
        return copy;
    }
}