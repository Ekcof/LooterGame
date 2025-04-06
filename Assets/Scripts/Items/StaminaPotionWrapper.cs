using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Stamina Potion", menuName = "Game Resources/Items/StaminaPotionWrapper")]
public class StaminaPotionWrapper : ItemWrapper<StaminaPotion>
{
}
[Serializable]
public class StaminaPotion : ItemBase<StaminaPotion>
{
    public override void OnDestroyItem(IItemContainer container)
    {
        throw new System.NotImplementedException();
    }

    public override void OnPut(IItemContainer container)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTake(IItemContainer container)
    {
        throw new System.NotImplementedException();
    }
}
