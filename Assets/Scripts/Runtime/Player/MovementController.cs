using PlazmaGames.Attribute;
using PlazmaGames.Core;

using UnityEngine;

using WrongFloor.MonoSystems;
using WrongFloor.Utilizes;

namespace WrongFloor.Player
{
    public enum MovementState
    {
        Grounded,
        Airborne
    }

    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        private readonly float GRAVITY = -9.8f;
        private readonly float GROUNDED_VEL = -2f;

        [SerializeField] private MovementSettings _settings;

        [Header("Debug")]
        [SerializeField, ReadOnly] private MovementState _state;
        [SerializeField, ReadOnly] private float _timeOffGround;
        [SerializeField, ReadOnly] private Vector3 _velocity;
        [SerializeField, ReadOnly] private Vector2 _horizontalVelocity;
        [SerializeField, ReadOnly] private Vector2 _movement;

        private CharacterController _controller;
        private ViewController _viewController;
        private IInputMonoSystem _input;

        private float Speed => _settings.Speed * (_crouched ? _settings.CrouchedMultiplier : 1.0f);
        public float GravityScale = 1.0f;
        private float Gravity => GRAVITY * _settings.GravityMul * GravityScale;

        private bool _crouched = false;

        public void Crouch()
        {
            _crouched = true;
            _viewController.Crouch();
            _controller.height = 2.0f - WFGameManager.Preferences.CrouchHeight;
            _controller.center = Vector3.zero.SetY(-WFGameManager.Preferences.CrouchHeight / 2.0f);
        }
        
        public void Uncrouch()
        {
            _crouched = false;
            _viewController.Uncrouch();
            _controller.height = 2.0f;
            _controller.center = Vector3.zero;
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _viewController = GetComponentInChildren<ViewController>();
            _input = GameManager.GetMonoSystem<IInputMonoSystem>();
        }

        private void Update()
        {
            if (WFGameManager.LockMovement || WFGameManager.IsPaused) return;
            _movement = Vector2.ClampMagnitude(_input.RawMovement, 1f);
            ApplyGravity();
            UpdateMovement();
            MoveController();
        }

        private void ApplyGravity()
        {
            _velocity.y = _state == MovementState.Airborne
                ? Mathf.MoveTowards(_velocity.y, -_settings.TerminalVelocity, -Gravity * Time.deltaTime)
                : GROUNDED_VEL * GravityScale;
        }

        private void UpdateHorizontalVelocity(float controlMultiplier)
        {
            Vector3 local = new Vector3(_movement.x, 0f, _movement.y);
            float multiplier = 1f;
            if (_movement.y < 0f) multiplier *= _settings.BackwardSpeedMul;
            if (_movement.x != 0f) multiplier *= _settings.StrafingSpeedMul;
            Vector3 target3D = transform.TransformDirection(local) * Speed * multiplier;
            Vector2 target2D = new Vector2(target3D.x, target3D.z) * controlMultiplier;

            _horizontalVelocity = Vector2.Lerp(
                _horizontalVelocity,
                target2D,
                _settings.InputSmoothing * Time.deltaTime
            );

            _velocity.x = _horizontalVelocity.x;
            _velocity.z = _horizontalVelocity.y;
        }

        private void UpdateGrounded()
        {
            UpdateHorizontalVelocity(1f);
            if (!_controller.isGrounded) _state = MovementState.Airborne;
        }

        private void UpdateAirborne()
        {
            UpdateHorizontalVelocity(_settings.AirControl);
            if (_controller.isGrounded)
            {
                _state = MovementState.Grounded;
                _velocity.y = GROUNDED_VEL * GravityScale;
            }
        }

        private void MoveController() => _controller.Move(_velocity * Time.deltaTime);

        private void UpdateMovement()
        {
            switch (_state)
            {
                case MovementState.Grounded: UpdateGrounded(); break;
                case MovementState.Airborne: UpdateAirborne(); break;
            }
        }

        public void Move(Vector3 d)
        {
            bool prev = _controller.enabled;
            _controller.enabled = false;
            transform.position += d;
            _controller.enabled = prev;
        }

        public void MoveToY(float y)
        {
            bool prev = _controller.enabled;
            _controller.enabled = false;
            transform.position = transform.position.SetY(y);
            _controller.enabled = prev;
        }
    }
}