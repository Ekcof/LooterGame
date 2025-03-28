using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySlotRow : MonoBehaviour
{
    [SerializeField] private InventorySlot[] _slots;
    public int SlotsNum => _slots.Length;

    public IEnumerable<IItem> SetItemViews(IEnumerable<IItem> items)
    {
        var temp = new List<InventorySlot>(_slots);
        var tempItems = new List<IItem>(items);
        foreach (var item in items)
        {
            var slot = temp.FirstOrDefault();
            if (slot != null)
            {
                slot.SetItemView(item);
                temp.Remove(slot);
                tempItems.Remove(item);
            }
            if (temp.Count == 0)
                break;
        }

        return tempItems;
    }


}
