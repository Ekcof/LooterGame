using System;
using UnityEngine;

public interface IItemWrapper
{
    string ID { get; }
}

[Serializable]
public abstract class ItemWrapper : ScriptableObject, IItemWrapper
{
    public abstract string ID { get; }
    public abstract ItemBase GetItemCopy(int count = 1);
}

public abstract class ItemWrapper<T> : ItemWrapper where T : ItemBase
{
    [SerializeField] private T _item;
    public sealed override string ID => _item.Id;

    public sealed override ItemBase GetItemCopy(int count = 1)
    {
        return _item.GetCopy(count);
    }
}
