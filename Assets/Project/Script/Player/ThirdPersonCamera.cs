using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;

    [Header("Rotación Cámara")]
    public float sensitivity = 0.15f;
    public float minVertical = -35f;
    public float maxVertical = 70f;

    [Header("Rotación Jugador")]
    [SerializeField] private float playerRotationSpeed = 10f;
    private InputAction lookAction;
    private PlayerControls controls;

    private Vector2 lookInput;

    private float yaw;
    private float pitch;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Awake()
    {
        controls = new PlayerControls();

        lookAction = controls.Player.Look;
    }


    private void OnEnable()
    {
        controls.Enable();
    }


    private void OnDisable()
    {
        controls.Disable();
    }


    private void LateUpdate()
    {

        if (player == null)
            return;


        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();


        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;


        pitch = Mathf.Clamp(
            pitch,
            minVertical,
            maxVertical
        );


        transform.rotation = Quaternion.Euler(
            pitch,
            yaw,
            0
        );


        // Girar jugador con la cámara
        Vector3 direction = transform.forward;
        direction.y = 0f;


        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);


            player.rotation = Quaternion.Slerp(
                player.rotation,
                targetRotation,
                playerRotationSpeed * Time.deltaTime
            );
        }
        Debug.Log(mouseDelta);
    }
}