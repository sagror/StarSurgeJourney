using UnityEngine;
using UnityEngine.InputSystem;
using StarSurgeJourney.Core.MVC;
using StarSurgeJourney.Models;
using StarSurgeJourney.Controllers;

namespace StarSurgeJourney.Controllers
{
    public class ShipController : BaseController
    {
        [SerializeField] private InputAction moveAction;
        [SerializeField] private InputAction rotateAction;
        [SerializeField] private InputAction fireAction;
        
        private ShipModel shipModel;
        private Vector2 moveInput;
        private float rotateInput;
        private bool fireInput;
        private WeaponController weaponController;
        
        private void Awake()
        {
            // Configure input actions if not assigned in inspector
            if (moveAction == null)
            {
                moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
                moveAction.AddCompositeBinding("Dpad")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
            }
            
            if (rotateAction == null)
            {
                rotateAction = new InputAction("Rotate", binding: "<Gamepad>/rightStick");
                rotateAction.AddCompositeBinding("1DAxis")
                    .With("Negative", "<Keyboard>/q")
                    .With("Positive", "<Keyboard>/e");
            }
            
            if (fireAction == null)
            {
                fireAction = new InputAction("Fire", binding: "<Gamepad>/rightTrigger");
                fireAction.AddBinding("<Keyboard>/space");
                fireAction.AddBinding("<Mouse>/leftButton");
            }
        }
        
        private void Start()
        {
            // Initialize MVC connection if not already done
            if (shipModel == null)
            {
                shipModel = GetComponent<ShipModel>();
                if (shipModel != null)
                {
                    Initialize(shipModel);
                    Debug.Log("ShipController initialized with ShipModel");
                }
                else
                {
                    Debug.LogError("ShipModel not found on the same GameObject!");
                }
            }
            
            // Get weapon controller
            weaponController = GetComponent<WeaponController>();
        }
        
        private void OnEnable()
        {
            moveAction.Enable();
            rotateAction.Enable();
            fireAction.Enable();
            
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            
            rotateAction.performed += OnRotate;
            rotateAction.canceled += OnRotate;
            
            fireAction.performed += OnFire;
            fireAction.canceled += OnFire;
            
            Debug.Log("Input actions enabled");
        }
        
        private void OnDisable()
        {
            moveAction.Disable();
            rotateAction.Disable();
            fireAction.Disable();
            
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            
            rotateAction.performed -= OnRotate;
            rotateAction.canceled -= OnRotate;
            
            fireAction.performed -= OnFire;
            fireAction.canceled -= OnFire;
        }
        
        public override void Initialize(BaseModel model)
        {
            base.Initialize(model);
            shipModel = model as ShipModel;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            // Handle the input based on the control that triggered the callback
            if (context.control != null)
            {
                // Get the raw value (float for 1D axis)
                float value = context.ReadValue<float>();
                
                // For vertical axis (W/S)
                if (context.control.name.Contains("w") || context.control.name.Contains("s"))
                {
                    moveInput.y = value; // W is usually positive for vertical axis
                }
                // For horizontal axis (A/D)
                else if (context.control.name.Contains("a") || context.control.name.Contains("d"))
                {
                    moveInput.x = value; // D is usually positive for horizontal axis
                }
            }
        }
        
        private void OnRotate(InputAction.CallbackContext context)
        {
            rotateInput = context.ReadValue<float>();
        }
        
        private void OnFire(InputAction.CallbackContext context)
        {
            fireInput = context.ReadValueAsButton();
            if (fireInput)
            {
                Debug.Log("Fire button pressed");
            }
        }
        
        public override void ProcessInput()
        {
            if (shipModel == null)
            {
                Debug.LogWarning("ShipModel is null in ProcessInput");
                return;
            }
            
            // Process movement (normalize to prevent diagonal movement being faster)
            Vector2 moveDirection = moveInput.normalized;
            
            // Apply movement to the ship model
            shipModel.Move(moveDirection, Time.deltaTime);
            
            // Process rotation
            shipModel.Rotate(rotateInput, Time.deltaTime);
            
            // Process firing
            if (fireInput && weaponController != null)
            {
                weaponController.Fire();
            }
        }
        
        protected override void UpdateModel()
        {
            // No additional implementation needed
            // as ProcessInput updates the model
        }
        
        private void Update()
        {
            ProcessInput();
        }
    }
}