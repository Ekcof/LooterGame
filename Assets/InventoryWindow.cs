using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : UIWindow
{
	[SerializeField] private ScrollRect _scrollRect;
	[SerializeField] private RectTransform _contentContainer;
	[SerializeField] private InventorySlotRow _rowPrefab;
	[SerializeField] private int _slotsPerRow = 4;

	private IItemContainer _itemContainer;
	private INPCInventory _inventory;
	private List<InventorySlotRow> _rows = new List<InventorySlotRow>();

	protected override void Awake()
	{
		base.Awake();

		// Configure ScrollRect to only scroll vertically
		if (_scrollRect != null)
		{
			_scrollRect.horizontal = false;
			_scrollRect.vertical = true;
		}
		else
		{
			Debug.LogWarning("ScrollRect is not assigned in InventoryWindow");
		}

		// Validate required components
		if (_contentContainer == null)
		{
			Debug.LogWarning("Content container is not assigned in InventoryWindow");
		}

		if (_rowPrefab == null)
		{
			Debug.LogWarning("Row prefab is not assigned in InventoryWindow");
		}

		// Ensure _slotsPerRow is at least 1
		_slotsPerRow = Mathf.Max(1, _slotsPerRow);
	}

	public void OpenInventory(IItemContainer container)
	{
		_itemContainer = container;
		_inventory = null;
		Open();
	}

	public void OpenInventory(INPCInventory container)
	{
		_itemContainer = null;
		_inventory = container;
		Open();
	}

	public override IUIWindow Open()
	{
		base.Open();

		if (_itemContainer != null || _inventory != null)
		{
			DisplayItems();
		}
		else
		{
			// Clear the inventory if no container is provided
			ClearRows();
		}

		return this;
	}

	private void DisplayItems()
	{
		// Clear existing rows
		ClearRows();

		List<ItemBase> items = null;

		if (_inventory != null)
		{
			items = _inventory.Containers.SelectMany(c => c.Items).ToList();
		}
		else
		if (_itemContainer is ItemHolder itemHolder)
		{
			items = itemHolder.Items.ToList();
		}
		// Create rows as needed
		EnsureRowsExist(items);

		// Distribute items to rows
		var remainingItems = new List<IItem>(items);

		for (int i = 0; i < _rows.Count && remainingItems.Count > 0; i++)
		{
			remainingItems = new List<IItem>(_rows[i].SetItemViews(remainingItems));
		}

	}

	private void ClearRows()
	{
		// Clear all slots in existing rows
		foreach (var row in _rows)
		{
			if (row != null)
			{
				foreach (var slot in row.GetComponentsInChildren<InventorySlot>())
				{
					if (slot != null)
					{
						slot.SetEmptyView();
					}
				}
			}
		}
	}

	private void EnsureRowsExist(List<ItemBase> items)
	{
		int totalItems = items.Count;
		int rowsNeeded = Mathf.CeilToInt((float)totalItems / _slotsPerRow);

		// Create additional rows if needed
		while (_rows.Count < rowsNeeded)
		{
			CreateNewRow();
		}

		// Enable only the rows we need
		for (int i = 0; i < _rows.Count; i++)
		{
			if (_rows[i] != null)
			{
				_rows[i].gameObject.SetActive(i < rowsNeeded);
			}
		}
	}

	private void CreateNewRow()
	{
		if (_rowPrefab != null && _contentContainer != null)
		{
			InventorySlotRow newRow = Instantiate(_rowPrefab, _contentContainer);
			_rows.Add(newRow);
		}
		else
		{
			Debug.LogError("Cannot create inventory row: prefab or container is missing");
		}
	}
}