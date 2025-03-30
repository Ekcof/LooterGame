using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed = 11.5f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float interactionDistance = 3.0f;

    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [Header("UI References")]
    [SerializeField] private InventoryWindow inventoryWindow;

    // Input System references
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    // Component references
    private CharacterController characterController;
    private ItemHolder playerInventory;

    // Movement variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    // Interaction variables
    private bool isInteracting = false;
    private ItemHolder currentTargetHolder = null;

    [HideInInspector]
    public bool canMove = true;

    private void Awake()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        playerInventory = GetComponent<ItemHolder>();

        // Setup Input System
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.defaultActionMap = "Player";
        }

        // Get input actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        interactAction = playerInput.actions["Interact"];

        // Setup interaction callback
        interactAction.performed += OnInteract;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }
            return;
        }

        // Handle movement
        HandleMovement();

        // Check for interactive objects
        CheckForInteractiveObjects();
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        // Get input values
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        bool isRunning = sprintAction.ReadValue<float>() > 0.1f;

        // Calculate movement direction
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = (isRunning ? runningSpeed : walkingSpeed) * moveInput.y;
        float curSpeedY = (isRunning ? runningSpeed : walkingSpeed) * moveInput.x;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Handle jumping
        if (jumpAction.triggered && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and camera rotation
        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
    }

    private void CheckForInteractiveObjects()
    {
        if (playerCamera == null) return;

        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            ItemHolder itemHolder = hit.collider.GetComponent<ItemHolder>();

            if (itemHolder != null && itemHolder != playerInventory)
            {
                currentTargetHolder = itemHolder;

                // Show interaction prompt (could be implemented with UI)
                Debug.Log("Press E to interact with item holder");
            }
            else
            {
                currentTargetHolder = null;
            }
        }
        else
        {
            currentTargetHolder = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer || currentTargetHolder == null) return;

        // Check if we're close enough to interact
        float distance = Vector3.Distance(transform.position, currentTargetHolder.transform.position);

        if (distance <= interactionDistance)
        {
            // If there's only one item, take it directly
            if (currentTargetHolder.ItemCount == 1)
            {
                TakeItemFromHolder(currentTargetHolder, 0);
            }
            // Otherwise open the inventory window
            else if (currentTargetHolder.ItemCount > 1)
            {
                OpenInventoryWindow(currentTargetHolder);
            }
        }
    }

    [Command]
    private void TakeItemFromHolder(ItemHolder holder, int itemIndex)
    {
        if (holder == null || playerInventory == null || playerInventory.IsFull)
            return;

        ItemBase item = holder.GetItem(itemIndex);

        if (item != null)
        {
            if (holder.RemoveItem(item))
            {
                playerInventory.AddItem(item);
            }
        }
    }

    private void OpenInventoryWindow(ItemHolder holder)
    {
        if (inventoryWindow != null)
        {
            // Unlock cursor for inventory interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Disable movement while inventory is open
            canMove = false;

            // Open the inventory window
            inventoryWindow.OpenInventory(holder);

            // Subscribe to window close event
            inventoryWindow.OnWindowClosed.AddListener(OnInventoryWindowClosed);
        }
    }

    private void OnInventoryWindowClosed()
    {
        // Re-lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Re-enable movement
        canMove = true;

        // Unsubscribe from event
        if (inventoryWindow != null)
        {
            inventoryWindow.OnWindowClosed.RemoveListener(OnInventoryWindowClosed);
        }
    }
}