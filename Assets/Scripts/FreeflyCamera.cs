using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Free-fly camera for shader inspection. WASD move, mouse look, Q/E or Space/C for up/down, Shift for speed.
/// Uses Input System with Player map (Move, Look, Jump, Crouch, Sprint).
/// </summary>
[RequireComponent(typeof(Camera))]
public class FreeflyCamera : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float sprintMultiplier = 2.5f;
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private bool lockCursorOnEnable = true;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private Vector2 _lookDelta;
    private float _pitch;
    private float _yaw;

    private void Awake()
    {
        if (inputActions != null)
        {
            var player = inputActions.FindActionMap("Player");
            _moveAction = player?.FindAction("Move");
            _lookAction = player?.FindAction("Look");
            _jumpAction = player?.FindAction("Jump");
            _crouchAction = player?.FindAction("Crouch");
            _sprintAction = player?.FindAction("Sprint");
        }
        Vector3 e = transform.eulerAngles;
        _pitch = e.x > 180f ? e.x - 360f : e.x;
        _yaw = e.y;
    }

    private void OnEnable()
    {
        inputActions?.Enable();
        if (lockCursorOnEnable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (_lookAction != null)
            _lookAction.performed += OnLookPerformed;
    }

    private void OnDisable()
    {
        if (_lookAction != null)
            _lookAction.performed -= OnLookPerformed;
        inputActions?.Disable();
        if (lockCursorOnEnable)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        _lookDelta += ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float speed = moveSpeed * (IsSprinting() ? sprintMultiplier : 1f);

        // Look
        Vector2 look = _lookDelta * lookSensitivity;
        _lookDelta = Vector2.zero;
        _yaw += look.x;
        _pitch = Mathf.Clamp(_pitch - look.y, -89f, 89f);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        // Move
        Vector2 move = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        float vertical = 0f;
        if (_jumpAction?.IsPressed() == true) vertical += 1f;
        if (_crouchAction?.IsPressed() == true) vertical -= 1f;

        Vector3 motion = transform.forward * move.y + transform.right * move.x + Vector3.up * vertical;
        if (motion.sqrMagnitude > 1f) motion.Normalize();
        transform.position += motion * (speed * dt);
    }

    private bool IsSprinting()
    {
        return _sprintAction?.IsPressed() == true;
    }
}
