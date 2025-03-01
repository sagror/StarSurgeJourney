using StarSurgeJourney.Core.MVC;
using StarSurgeJourney.Models;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        private void Awake()
        {
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
            moveInput = context.ReadValue<Vector2>();
        }
        
        private void OnRotate(InputAction.CallbackContext context)
        {
            rotateInput = context.ReadValue<float>();
        }
        
        private void OnFire(InputAction.CallbackContext context)
        {
            fireInput = context.ReadValueAsButton();
        }
        
        public override void ProcessInput()
        {
            if (shipModel == null)
                return;
                
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            shipModel.Move(moveDirection, Time.deltaTime);
            
            shipModel.Rotate(rotateInput, Time.deltaTime);
            
            if (fireInput)
            {
                shipModel.Fire();
            }
        }
        
        protected override void UpdateModel()
        {
            
        }
        
        private void Update()
        {
            ProcessInput();
        }
    }
}