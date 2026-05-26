using UnityEngine;

public class SimpleFlyCamera : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float lookSpeed = 2f;
    float pitch = 0f;
    float yaw = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (transform.forward * v + transform.right * h) * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * lookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}