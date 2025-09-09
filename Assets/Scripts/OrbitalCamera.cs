using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    private float _yMinLimit = -20f;
    private float _yMaxLimit = 80f;

    private float _x = 0.0f;
    private float _y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (target)
        {
            _x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            _y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            _y = ClampAngle(_y, _yMinLimit, _yMaxLimit);

            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}