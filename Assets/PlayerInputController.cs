using Mirror;
using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : NetworkBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] private float _walkingSpeed = 7.5f;
	[SerializeField] private float _runningSpeed = 11.5f;
	[SerializeField] private float _jumpSpeed = 8.0f;
	[SerializeField] private float _gravity = 20.0f;
	[SerializeField] private float _interactionDistance = 3.0f;

	[Header("Camera Settings")]
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private float _lookSpeed = 2.0f;
	[SerializeField] private float _lookXLimit = 45.0f;

	[SerializeField] private NPCInventory _playerInventory;

	[Inject] private InventoryWindow _inventoryWindow;

	// Input System references
	private PlayerInput _playerInput;
	private InputAction _moveAction;
	private InputAction _lookAction;
	private InputAction _jumpAction;
	private InputAction _sprintAction;
	private InputAction _interactAction;
	private InputAction _inventoryAction;

	// Component references
	private CharacterController _characterController;

	// Movement variables
	private Vector3 _moveDirection = Vector3.zero;
	private float _rotationX = 0;

	// Interaction variables
	private bool _isInteracting = false;
	private ItemHolder _currentTargetHolder = null;

	[HideInInspector]
	public bool _canMove = true;

	private void Awake()
	{
		Debug.Log($"_____Awake {gameObject.name} {nameof(PlayerInputController)}");
		// Get components
		_characterController = GetComponent<CharacterController>();

		// Setup Input System
		_playerInput = GetComponent<PlayerInput>();
		if (_playerInput == null)
		{
			_playerInput = gameObject.AddComponent<PlayerInput>();
			_playerInput.defaultActionMap = "Player";
		}

		// Get input actions
		_moveAction = _playerInput.actions["Move"];
		_lookAction = _playerInput.actions["Look"];
		_jumpAction = _playerInput.actions["Jump"];
		_sprintAction = _playerInput.actions["Sprint"];
		_interactAction = _playerInput.actions["Interact"];
		_inventoryAction = _playerInput.actions["Inventory"];

		// Setup interaction callback
		_interactAction.performed += OnInteractPressed;

		_inventoryAction.performed += OnInventoryPressed;
	}

	private void OnInventoryPressed(InputAction.CallbackContext context)
	{
		_isInteracting = true;
		if (_inventoryWindow == null)
		{
			UnityEngine.Debug.Log($"No _inventoryWindow in container");
			return;
		}
		if (_playerInventory == null)
		{
			UnityEngine.Debug.Log($"No _playerInventory found");
			return;
		}
		_inventoryWindow.OpenInventory(_playerInventory);
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

		if (_playerCamera != null)
		{
			_playerCamera.gameObject.SetActive(true);
		}

		// Lock cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnDestroy()
	{
		// Unsubscribe from events
		if (_interactAction != null)
		{
			_interactAction.performed -= OnInteractPressed;
		}
	}

	private void Update()
	{
		if (UIWindow.IsActive)
			return;

		if (!_isLocalPlayer)
		{
			if (_playerCamera != null)
			{
				_playerCamera.gameObject.SetActive(false);
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
		if (!_canMove) return;

		// Get input values
		Vector2 moveInput = _moveAction.ReadValue<Vector2>();
		Vector2 lookInput = _lookAction.ReadValue<Vector2>();
		bool isRunning = _sprintAction.ReadValue<float>() > 0.1f;

		// Calculate movement direction
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		Vector3 right = transform.TransformDirection(Vector3.right);

		float curSpeedX = (isRunning ? _runningSpeed : _walkingSpeed) * moveInput.y;
		float curSpeedY = (isRunning ? _runningSpeed : _walkingSpeed) * moveInput.x;

		float movementDirectionY = _moveDirection.y;
		_moveDirection = (forward * curSpeedX) + (right * curSpeedY);

		// Handle jumping
		if (_jumpAction.triggered && _characterController.isGrounded)
		{
			_moveDirection.y = _jumpSpeed;
		}
		else
		{
			_moveDirection.y = movementDirectionY;
		}

		// Apply gravity
		if (!_characterController.isGrounded)
		{
			_moveDirection.y -= _gravity * Time.deltaTime;
		}

		// Move the controller
		_characterController.Move(_moveDirection * Time.deltaTime);

		// Player and camera rotation
		_rotationX += -lookInput.y * _lookSpeed;
		_rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);

		if (_playerCamera != null)
		{
			_playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
		}

		transform.rotation *= Quaternion.Euler(0, lookInput.x * _lookSpeed, 0);
	}

	private void CheckForInteractiveObjects()
	{
		if (_playerCamera == null) return;

		RaycastHit hit;
		Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

		if (Physics.Raycast(ray, out hit, _interactionDistance))
		{
			ItemHolder itemHolder = hit.collider.GetComponent<ItemHolder>();

			if (itemHolder != null && itemHolder != _playerInventory)
			{
				_currentTargetHolder = itemHolder;

				// Show interaction prompt (could be implemented with UI)
				//Debug.Log("Press E to interact with item holder");
			}
			else
			{
				_currentTargetHolder = null;
			}
		}
		else
		{
			_currentTargetHolder = null;
		}
	}

	private void OnInteractPressed(InputAction.CallbackContext context)
	{
		if (!_isLocalPlayer || _currentTargetHolder == null) return;

		Debug.Log($"_____Is pressed {context.action.name} {_currentTargetHolder.name}");
		// Check if we're close enough to interact
		float distance = Vector3.Distance(transform.position, _currentTargetHolder.transform.position);

		if (distance <= _interactionDistance)
		{
			Debug.Log($"_____Is pressed Distance is  enough {distance}");
			// If there's only one item, take it directly
			if (_currentTargetHolder.ItemCount == 1)
			{
				Debug.Log($"_____Is pressed Take the item {distance}");
				TakeItemFromHolder(_currentTargetHolder, 0);
			}
			// Otherwise open the inventory window
			else if (_currentTargetHolder.ItemCount > 1)
			{
				Debug.Log($"_____Is pressed Open window with {_currentTargetHolder.ItemCount} items {distance}");
				OpenInventoryWindow(_currentTargetHolder);
			}

		}
		else
		{
			Debug.Log($"_____Is pressed Distance is not enought {distance}");
		}

	}

	[Command]
	private void TakeItemFromHolder(ItemHolder holder, int itemIndex)
	{
		Debug.Log($"TakeItemFromHolder 0 playerInventory == null {_playerInventory == null}");
		if (holder == null || _playerInventory == null || _playerInventory.IsFull)
			return;
		Debug.Log($"TakeItemFromHolder 1");
		ItemBase item = holder.GetItem(itemIndex);
		Debug.Log($"TakeItemFromHolder 2");
		if (item != null)
		{
			Debug.Log($"TakeItemFromHolder 3");
			if (_playerInventory.TryAddItem(item))
			{
				holder.TryRemoveItem(item);
				Debug.Log($"TakeItemFromHolder 4");
			}
		}
	}


	private void OpenInventoryWindow(ItemHolder holder)
	{
		if (_inventoryWindow != null)
		{
			// Unlock cursor for inventory interaction
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			// Disable movement while inventory is open
			_canMove = false;

			// Open the inventory window
			_inventoryWindow.OpenInventory(holder);

			// Subscribe to window close event
			_inventoryWindow.OnWindowClosed.AddListener(OnInventoryWindowClosed);
		}
	}

	private void OnInventoryWindowClosed()
	{
		// Re-lock cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Re-enable movement
		_canMove = true;

		// Unsubscribe from event
		if (_inventoryWindow != null)
		{
			_inventoryWindow.OnWindowClosed.RemoveListener(OnInventoryWindowClosed);
		}
	}
}