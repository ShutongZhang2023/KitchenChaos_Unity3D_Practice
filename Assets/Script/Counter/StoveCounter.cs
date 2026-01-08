using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BasicCounter, IHasProgress
{
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipes;
    [SerializeField] private BurningRecipeSO[] burningRecipes;

    public event EventHandler<IHasProgress.OnProgressChangedArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    private State state;
    private float fryingTimer;
    private float friedTimer;
    private FryingRecipeSO fryingRecipe;
    private BurningRecipeSO burningRecipe;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        switch (state) {
            case State.Idle:
                break;
            case State.Frying:
                if (HasKitchenObject())
                {
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipe.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipe.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);

                        friedTimer = 0f;
                        burningRecipe = GetBuriningRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());

                        state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    }
                }
                break;
            case State.Fried:
                if (HasKitchenObject())
                {
                    friedTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedArgs
                    {
                        progressNormalized = friedTimer / burningRecipe.burningTimerMax
                    });

                    if (friedTimer > burningRecipe.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipe.output, this);

                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
                break;
            case State.Burned:
                break;
        }

    }

    public override void Interact(Player player)
    {
        //pick up and put object

        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipe = GetFryingRecipeWithInput(GetKitchenObject().GetKitchenObjectSO());
                    
                    //when drop item, start frying
                    fryingTimer = 0f;
                    state = State.Frying;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipe.fryingTimerMax
                    });
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipe = GetFryingRecipeWithInput(input);
        if (fryingRecipe != null)
        {
            return fryingRecipe.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipe = GetFryingRecipeWithInput(input);
        return fryingRecipe != null;
    }

    private FryingRecipeSO GetFryingRecipeWithInput(KitchenObjectSO input)
    {
        foreach (FryingRecipeSO fryingRecipe in fryingRecipes)
        {
            if (fryingRecipe.input == input)
            {
                return fryingRecipe;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBuriningRecipeWithInput(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO burningRecipe in burningRecipes)
        {
            if (burningRecipe.input == input)
            {
                return burningRecipe;
            }
        }
        return null;
    }
}
