using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using WrongFloor.UI;

namespace WrongFloor.MonoSystems
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputMonoSystem : MonoBehaviour, IInputMonoSystem
    {
        [SerializeField] private PlayerInput _input;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _interactAction;
        public UnityEvent InteractCallback { get; private set; }

        public Vector2 RawMovement { get; private set; }
        public Vector2 RawLook { get; private set; }

        private void Awake()
        {
            if (!_input) _input = GetComponent<PlayerInput>();

            InteractCallback = new UnityEvent();

            _moveAction = _input.actions["Move"];
            _lookAction = _input.actions["Look"];
            _interactAction = _input.actions["Interact"];

            _moveAction.performed += HandleMoveAction;
            _lookAction.performed += HandleLookAction;
            _interactAction.performed += HandleInteractAction;
        }

        private void OnDestroy()
        {
            _moveAction.performed -= HandleMoveAction;
            _lookAction.performed -= HandleLookAction;
            _interactAction.performed -= HandleInteractAction;
        }

        private void HandleMoveAction(InputAction.CallbackContext e)
        {
            RawMovement = e.ReadValue<Vector2>();
        }

        private void HandleLookAction(InputAction.CallbackContext e)
        {
            RawLook = e.ReadValue<Vector2>();
        }

        private void HandleInteractAction(InputAction.CallbackContext e)
        {
            InteractCallback.Invoke();
        }

        public void EnableMovement()
        {
            _moveAction.Enable();
        }

        public void DisableMovement()
        {
            RawMovement = Vector2.zero;
            _moveAction.Disable();
        }

        public void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<GameView>()) GameManager.GetMonoSystem<IUIMonoSystem>().Show<PausedView>();
                else if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<PausedView>()) GameManager.GetMonoSystem<IUIMonoSystem>().GetView<PausedView>().Resume();
            }
        }
    }
}