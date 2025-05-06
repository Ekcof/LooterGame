using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingPotion", menuName = "Game Resources/Items/HealingPotionWrapper")]
public class HealingPotionWrapper : ItemWrapper<HealingPotion>
{

}
[Serializable]
public class HealingPotion : ItemBase<HealingPotion>
{
    public override void OnDestroyItem(IItemContainer container)
    {
        //
    }

    public override void OnPut(IItemContainer container)
    {
        //
    }

    public override void OnTake(IItemContainer container)
    {
        //
    }
}