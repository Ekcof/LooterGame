using UnityEngine;

public interface IItemWrapper
{
    string ID { get; }
}

public abstract class ItemWrapper : ScriptableObject, IItemWrapper
{
    public abstract string ID { get; }
    public abstract ItemBase GetItemCopy();
}

public abstract class ItemWrapper<T> : ItemWrapper where T : ItemBase
{
    [SerializeField] private T _item;
    public sealed override string ID => _item.Id;

    public sealed override ItemBase GetItemCopy()
    {
        return _item.GetCopy();
    }
}
