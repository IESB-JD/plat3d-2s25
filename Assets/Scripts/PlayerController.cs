using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 50f;
    public Rigidbody rb;
    public CharacterController controller;
    private float _moveX;
    private float _moveY;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); //cache
    }

    void Update()
    {
        _moveX = Input.GetAxis("Horizontal");
        _moveY = Input.GetAxis("Vertical");
    }
    
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(_moveX, 0.0f, _moveY).normalized;
        //rb.velocity = movement * speed;
        controller.Move(movement * speed);
    }
    
    
    
    
}
