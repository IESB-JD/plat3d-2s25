using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController cc;
    public float speed = 5f;
    public float speedRunning = 15f;
    public float currentSpeed = 0f;
    
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    
    public float rotationSpeed = 100f;
    
    public Vector3 verticalVelocity;
    public bool isGrounded;
    
    public Transform groundCheck;
    public LayerMask groundMask;

    private void Update()
    {
        DetectGround();
        MoveCharacter();
        ApplyGravity();
    }
    
    private void DetectGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position,
            groundDistance,
            groundMask);
        
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
    }
    
    private void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);
        Vector3 moveDirection = transform.forward * vertical;
        
        currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = speedRunning;
        }
        
        cc.Move(moveDirection * currentSpeed * Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        cc.Move(verticalVelocity * Time.deltaTime);
    }
}