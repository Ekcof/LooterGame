using Mirror;
using Mirror.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : NetworkBehaviour, IItemContainer
{
	[Serializable]
	private class ItemToAdd
	{
		public ItemWrapper ItemWrapper;
		[Min(1)]
		public int Count;
	}


	[SerializeField] private int _capacity = 10;
	// TODO: Set a field with preset items which would be added at Awake
	[SerializeField]
	private ItemToAdd[] _itemsToAdd;

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

	[Server]
	protected virtual void Awake()
	{
		Debug.Log($"_____Awake");
		foreach (var itemWrapper in _itemsToAdd)
		{
			var item = itemWrapper.ItemWrapper.GetItemCopy(itemWrapper.Count);
			Debug.Log($"_____Add item {itemWrapper.ItemWrapper.ID} with count {itemWrapper.Count}_____");
			TryAddItem(item);
		}
	}

	#endregion


	[Server]
	public bool TryAddItem(ItemBase item)
	{
		if (item == null || IsFull)
			return false;

		_items.Add(item);
		_totalWeight += item.TotalWeight;
		item.OnPut(this);

		return true;
	}

	[Server]
	public bool TryRemoveItem(ItemBase item)
	{
		if (item == null || !_items.Contains(item))
			return false;

		_totalWeight -= item.TotalWeight;
		_items.Remove(item);
		item.OnTake(this);

		if (_items.Count == 0)
		{
			Destroy();
		}

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

	[Server]

	public void Destroy()
	{
		Clear();
		NetworkServer.Destroy(gameObject);
	}

	#region Client Methods

	// Add client-specific methods if needed

	#endregion

	#region Network Callbacks

	public override void OnStartServer()
	{
		base.OnStartServer();
		Debug.Log("_____OnStartServer");
		foreach (var itemWrapper in _itemsToAdd)
		{
			var item = itemWrapper.ItemWrapper.GetItemCopy(itemWrapper.Count);
			Debug.Log($"_____Add item {itemWrapper.ItemWrapper.ID} with count {itemWrapper.Count}_____");
			TryAddItem(item);
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		// Client-specific initialization
	}

	#endregion
}