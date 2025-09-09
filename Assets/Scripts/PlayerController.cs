using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 80f;
    public Rigidbody rb;
    
    private float _moveX;
    private float _moveZ;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _moveX = Input.GetAxis("Horizontal");
        _moveZ = Input.GetAxis("Vertical");
    }
    
    private void FixedUpdate()
    {
        Vector3 move = transform.forward * _moveZ * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        Quaternion turn = Quaternion.Euler(0f, _moveX * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private interface IInteractable
    {
        public void Interact();
    }
}