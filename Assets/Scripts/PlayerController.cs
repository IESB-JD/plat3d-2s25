using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public CharacterController cc;
    public float speed = 5f;
    public float speedRunning = 15f;
    public float currentSpeed = 0f;
    
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    
    public float rotationSpeed = 100f;
    
    public Vector3 verticalVelocity;
    public bool isGrounded;
    
    public Transform groundCheck;
    public LayerMask groundMask;
    
    private int _cristalAmount = 0;
    
    public Transform teleportTarget;

    public static event Action OnPlayerDied;
    public static event Action<int> OnCristalCollected;
    public static event Action<float> OnHpChanged;

    #region Damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (OnHpChanged != null)
        {
            OnHpChanged.Invoke(currentHealth);
        }
        Debug.Log($"Tomou {damage} de dano e agora está com {currentHealth}");
        if (currentHealth <= 0)
        {
            Debug.Log("Morreu");
            if (OnPlayerDied != null)
            {
                OnPlayerDied.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Collect(other);
        }

        if (other.CompareTag("Portal"))
        {
            Teleport();
        }
    }
    #endregion

    
    private void Collect(Collider other)
    {
        other.gameObject.SetActive(false);
        if (OnCristalCollected != null)
        {
            _cristalAmount = _cristalAmount + 1;
            OnCristalCollected.Invoke(_cristalAmount);
        }
    }

    private void Teleport()
    { 
        cc.enabled = false;
        transform.position = teleportTarget.position;
        cc.enabled = true;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(!cc.enabled) return;
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
        
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