using Mirror;
using System.Collections.Generic;

public interface INPCInventory
{
    IEnumerable<IItemContainer> Containers { get; }
}

public class NPCInventory : NetworkBehaviour, INPCInventory
{
    public IEnumerable<IItemContainer> Containers => throw new System.NotImplementedException();
}
