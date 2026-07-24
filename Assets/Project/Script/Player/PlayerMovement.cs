using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform modelPivot;

    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;

    [Header("Rotación modelo")]
    [SerializeField] private float modelRotationSpeed = 10f;


    private CharacterController controller;
    private PlayerControls controls;

    private Vector2 moveInput;
    private bool isRunning;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
        };

        controls.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
        };


        controls.Player.Sprint.performed += ctx =>
        {
            isRunning = true;
        };

        controls.Player.Sprint.canceled += ctx =>
        {
            isRunning = false;
        };
    }


    private void OnEnable()
    {
        controls.Enable();
    }


    private void OnDisable()
    {
        controls.Disable();
    }


    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }


    private void Update()
    {
        if (cameraTransform == null)
            return;


        // Dirección de la cámara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;


        // Evitar movimiento vertical
        forward.y = 0f;
        right.y = 0f;


        forward.Normalize();
        right.Normalize();


        // Movimiento relativo a cámara
        Vector3 direction =
            forward * moveInput.y +
            right * moveInput.x;


        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }


        // Rotar SOLO el ModelPivot
        if (modelPivot != null && direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);


            modelPivot.rotation = Quaternion.Slerp(
                modelPivot.rotation,
                targetRotation,
                modelRotationSpeed * Time.deltaTime
            );
        }


        // Velocidad
        float speed = isRunning ? runSpeed : walkSpeed;


        // Movimiento del Player
        controller.Move(
            direction * speed * Time.deltaTime
        );
    }
}