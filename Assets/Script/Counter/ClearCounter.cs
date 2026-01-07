using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BasicCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        //just pick up and put object
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                //drop this object on counter
                player.GetKitchenObject().SetKitchenObjectParent(this); 
            }
        }
        else {
            //there is kitchen object on counter
            if (!player.HasKitchenObject())
            {
                //pick up object from counter
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
