using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySlotRow : MonoBehaviour
{
    [SerializeField] private InventorySlot[] _slots;
    public int SlotsNum => _slots.Length;

    public IEnumerable<IItem> SetItemViews(IEnumerable<IItem> items)
    {
        Debug.Log($"_____Set views for {items.Count()} items");   
        var temp = new List<InventorySlot>(_slots);
        var tempItems = new List<IItem>(items);
        foreach (var item in items)
        {
            var slot = temp.FirstOrDefault();
            if (slot != null)
            {
				Debug.Log($"_____Set views for {item.Id} in slot {slot.name}");
				slot.SetItemView(item);
                temp.Remove(slot);
                tempItems.Remove(item);
            }
            if (temp.Count == 0)
                break;
        }

        if (temp.Count > 0)
        {
            foreach (var item in temp)
            {
                item.SetEmptyView();
            }
        }

        return tempItems;
    }


}
