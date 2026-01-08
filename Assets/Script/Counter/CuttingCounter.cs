using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CuttingCounter : BasicCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;
    public override void Interact(Player player)
    {
        //pick up and put object
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                    player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //cutting logic
            KitchenObjectSO cutkitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(cutkitchenObjectSO, this);
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipes)
        {
            if (cuttingRecipe.input == input)
            {
                return cuttingRecipe.output;
            }
        }
        return null;
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipes)
        {
            if (cuttingRecipe.input == input)
            {
                return true;
            }
        }
        return false;
    }
}
