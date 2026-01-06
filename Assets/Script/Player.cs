using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    private void HandleMovement() 
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            //try only x movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                //try only z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }

        }

        if (canMove)
        {
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, moveDistance);  //rotation
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
}