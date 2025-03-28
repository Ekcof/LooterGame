public class InventoryWindow : UIWindow
{
    private IItemContainer _itemContainer;

    public void OpenInventory(IItemContainer container)
    {
        _itemContainer = container;
    }

    public override IUIWindow Open()
    {
        return null;
    }
}
