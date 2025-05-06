using System;
using System.Collections.Generic;
using System.Linq;

public interface IItemContainer
{
	bool IsFull { get; }
	int Capacity { get; }
	bool TryAddItem(ItemBase item);
	bool TryRemoveItem(ItemBase item);
}

[Serializable]
public class ItemContainer : IItemContainer
{
	public bool IsFull => Capacity == 0;
	public int Capacity => _totalSlots - _items.Sum(s => s.SlotSize);
	private List<ItemBase> _items = new List<ItemBase>();
	private int _totalSlots = 10; // Default slots


	public bool TryAddItem(ItemBase item)
	{
		_items.Add(item);
		return true;
	}

	public bool TryRemoveItem(ItemBase item)
	{
		throw new NotImplementedException();
	}
}
