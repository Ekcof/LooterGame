using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Disposables Holder", menuName = "Items/DisposablesHolder")]
public class DisposablesHolder : ScriptableObject, IItemHolder
{
    [SerializeField] private ItemWrapper[] _items;

    public IItem GetItem(string id)
    {
        var item = _items.FirstOrDefault(i => i.ID.Equals(id));

        if (item == null || item == default)
            throw new NullReferenceException(message: $"No item {id} found");

        return item.GetItemCopy();
    }
}
