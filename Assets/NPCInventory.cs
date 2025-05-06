using Mirror;
using System.Collections.Generic;
using System.Linq;

public interface INPCInventory
{
    IEnumerable<IItemContainer> Containers { get; }
    bool TryAddItem(ItemBase item);
    void RemoveItem(ItemBase item);
	bool HasCapacity(params ItemBase[] items);
	/// <summary>
	/// Check if the inventory is full
	/// </summary>
	/// <returns></returns>
	bool IsFull { get; }
}

public class NPCInventory : NetworkBehaviour, INPCInventory
{
    public IEnumerable<IItemContainer> Containers { get; private set; }

    public bool IsFull => false; // TODO: Implement this property to check if the inventory is full


	private void Awake()
	{
		ItemContainer container = new ItemContainer();

		Containers = new List<IItemContainer> { container };
	}

	public bool TryAddItem(ItemBase item)
	{
		if (HasCapacity(item))
		{
			foreach (var container in Containers)
			{
				if (container.Capacity >= item.SlotSize)
				{
					container.TryAddItem(item);
					return true;
				}
			}
			return true;
		}
		return false;
	}

	[Command]
	public void RemoveItem(ItemBase item)
	{
		// TODO: Implement the logic to remove an item from the inventory
	}

	public bool HasCapacity(params ItemBase[] items)
	{
		int allContainersSlots = Containers.Sum(c => c.Capacity);
		int allItemsSlots = items.Sum(i => i.SlotSize);
		bool hasItemBiggerThanMaximalCapableContainer = items.Any(i => i.SlotSize > Containers.Max(c => c.Capacity));

		return !hasItemBiggerThanMaximalCapableContainer && allContainersSlots >= allItemsSlots;
	}
}
