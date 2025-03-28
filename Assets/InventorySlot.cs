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

    }
}
