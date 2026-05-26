using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float mouseSensitivity = 2f;
    public float gravity = -20f;
    [Header("Camera")]
    [SerializeField, Range(0.001f, 0.3f)] private float nearClipDistance = 0.01f;
    [Header("Debug")]
    [SerializeField] private bool logBlockingColliders = true;
    [SerializeField, Min(0.05f)] private float colliderLogInterval = 0.35f;

    private CharacterController controller;
    private Transform cam;
    private float pitch = 0f;
    private float verticalVelocity = 0f;
    private float lastColliderLogTime = -10f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.nearClipPlane = nearClipDistance;
            cam = mainCam.transform;
        }
        else
        {
            Debug.LogError("PlayerController: nenhuma Camera com tag MainCamera encontrada.");
            enabled = false;
            return;
        }

        if (controller == null)
        {
            Debug.LogError("PlayerController: CharacterController ausente no objeto do player.");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameInput.GetCursorUnlockDown())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (GameInput.GetCursorRelockDown())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Vector2 lookDelta = GameInput.GetLookDelta();
        float mouseX = lookDelta.x * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = lookDelta.y * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        cam.localEulerAngles = new Vector3(pitch, 0f, 0f);

        Vector2 moveInput = GameInput.GetMoveVector();
        float h = moveInput.x;
        float v = moveInput.y;

        Vector3 move = (transform.right * h + transform.forward * v) * moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!logBlockingColliders || hit.collider == null) return;
        if (Time.time - lastColliderLogTime < colliderLogInterval) return;

        if (hit.moveDirection.y < -0.6f) return;

        lastColliderLogTime = Time.time;
        Debug.Log($"Bloqueio detectado: {hit.collider.name} (tag={hit.collider.tag}, layer={LayerMask.LayerToName(hit.collider.gameObject.layer)})");
    }
}