using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TextUI _title;
    [SerializeField] private TextUI _numText;
    [SerializeField] private Image _image;

    public void SetEmptyView()
    {
        _title.SetEmpty();
        _numText.SetEmpty();
        _image.enabled = false;
    }

    public void SetItemView(IItem item)
    {
        if (item == null)
        {
            SetEmptyView();
            return;
        }

        // Set the item name
        _title.SetText(item.Id);
        
        // Set the item amount if it's stackable
        if (item.IsStackable && item.Amount > 1)
        {
            _numText.SetText(item.Amount.ToString());
        }
        else
        {
            _numText.SetEmpty();
        }
        
        // Set the item image
        if (item.SpritePreview != null)
        {
            _image.sprite = item.SpritePreview;
            _image.enabled = true;
        }
        else
        {
            _image.enabled = false;
        }
    }
}