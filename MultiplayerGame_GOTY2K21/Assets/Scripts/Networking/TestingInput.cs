using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//TENIR EN COMPTE LA RESOLUTION DE LA SCREEN PER LA SENSIBILITAT DEL MOUSE
public class TestingInput : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float characterSpeed = 12f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController controller;
    private InputManager playerInput;
    private float xRotation = 0f;
    private float velY = 0f;
    private bool isGrounded;

    //[SerializeField] private Vector2 sensitivity;
    //[SerializeField] private Vector2 acceleration;
    //private Vector2 velocity = new Vector2(0, 0);
    //private Vector2 rotation = new Vector2(0, 0);
    private void Awake()
    {
        playerInput = new InputManager();
        //playerInput.Player.Enable();
        playerInput.Player.Jump.performed += Jump;
        //playerInput.Player.Rotation.performed += Rotate;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Rotate();

        if (!gameObject.GetComponent<PlayerHealthManager>().playerDead)
        {
           UpdateMove();
        }
    }
    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
            velY = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    public void Rotate()
    {
        //Problem scales with deltas in new iput system !!!!
        //Temporary solution: multiply by 0.5 * 0.1 (scales on winodws) Done it on the actions processors
        //Link: https://forum.unity.com/threads/mouse-delta-input.646606/
        //Debug.Log("Axis: " + Input.GetAxis("Mouse X") + ", " + Input.GetAxis("Mouse Y"));
        //Debug.Log("Newsys: " + Mouse.current.delta.ReadValue().x * 0.5f * 0.1f + ", " + Mouse.current.delta.ReadValue().y * 0.5f * 0.1f);
        float mouseX = playerInput.Player.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = playerInput.Player.Look.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;
        //Debug.Log(mouseX + ", " + mouseY);
    
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    //public void Rotate2()
    //{
    //    Vector2 wantedVelocity = Mouse.current.delta.ReadValue() * sensitivity;
    //    Debug.Log(wantedVelocity);
    //    velocity = new Vector2(Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime), Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime));
    //    rotation += velocity * Time.deltaTime;
    //    rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
    //    cameraTransform.localEulerAngles = new Vector3(rotation.y, 0, 0);
    //    transform.localEulerAngles = new Vector3(0, rotation.x, 0);
    //}
    private void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velY < 0f)
            velY = -2f;

        float x = playerInput.Player.Movement.ReadValue<Vector2>().x;
        float z = playerInput.Player.Movement.ReadValue<Vector2>().y;
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        controller.Move(moveDirection * characterSpeed * Time.deltaTime);

        velY += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f,velY,0f) * Time.deltaTime);
    }
}
