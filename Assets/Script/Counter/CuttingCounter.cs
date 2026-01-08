using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CuttingCounter : BasicCounter
{
    public event EventHandler OnEachCut;
    public event EventHandler<onCutEventArgs> OnCut;
    public class onCutEventArgs : EventArgs
    {
        public float progressNormalized;
    }

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        //pick up and put object
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnCut?.Invoke(this, new onCutEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax
                    });

                }
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
            //cutting logic: there is an object to cut  (and able to cut)
            cuttingProgress++;

            OnEachCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnCut?.Invoke(this, new onCutEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipe.cuttingProgressMax)
            {
                //cut
                KitchenObjectSO cutkitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(cutkitchenObjectSO, this);
            }
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(input);
        if (cuttingRecipe != null)
        {
            return cuttingRecipe.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(input);
        return cuttingRecipe != null;
    }

    private CuttingRecipeSO GetCuttingRecipeWithInput(KitchenObjectSO input) {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipes)
        {
            if (cuttingRecipe.input == input)
            {
                return cuttingRecipe;
            }
        }
        return null;
    }
}
