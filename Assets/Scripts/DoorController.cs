using UnityEngine;
using UnityEngine.Serialization;

public class DoorController : MonoBehaviour
{
    public enum DoorOpenMode
    {
        SlideUp,
        SlideLocal,
        HingeRotate
    }

    [Header("Door Motion")]
    [SerializeField] private DoorOpenMode openMode = DoorOpenMode.SlideLocal;
    [FormerlySerializedAs("openHeight")]
    public float openDistance = 2.2f;
    public float openSpeed = 2f;
    [SerializeField] private Vector3 localSlideDirection = Vector3.right;
    [SerializeField] private Transform hingePivot;
    [SerializeField] private float hingeOpenAngle = 95f;
    [Header("Optional Open Extras")]
    [SerializeField] private GameObject[] objectsToEnableWhenOpen;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openDoorSound;

    [Header("Feedback")]
    [SerializeField] private GameManager gameManager;
    [TextArea]
    [SerializeField] private string lockedDoorHint = "A porta está trancada. Descubra o código!";

    private bool isOpen = false;
    private bool isLocked = true;
    private Vector3 closedWorldPos;
    private Vector3 openWorldPos;
    private Vector3 closedLocalPos;
    private Vector3 openLocalPos;
    private Quaternion closedHingeLocalRotation;
    private Quaternion openHingeLocalRotation;
    private bool lastAppliedOpenState;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = Object.FindFirstObjectByType<GameManager>();
        }

        CacheClosedState();
        ApplyPassageBlockers(isOpen);
        lastAppliedOpenState = isOpen;
    }

    void Update()
    {
        float t = Time.deltaTime * openSpeed;

        switch (openMode)
        {
            case DoorOpenMode.SlideUp:
            {
                Vector3 target = isOpen ? openWorldPos : closedWorldPos;
                transform.position = Vector3.Lerp(transform.position, target, t);
                break;
            }
            case DoorOpenMode.SlideLocal:
            {
                Vector3 target = isOpen ? openLocalPos : closedLocalPos;
                transform.localPosition = Vector3.Lerp(transform.localPosition, target, t);
                break;
            }
            case DoorOpenMode.HingeRotate:
            {
                if (hingePivot == null) return;
                Quaternion target = isOpen ? openHingeLocalRotation : closedHingeLocalRotation;
                hingePivot.localRotation = Quaternion.Slerp(hingePivot.localRotation, target, t);
                break;
            }
        }

        if (lastAppliedOpenState != isOpen)
        {
            ApplyPassageBlockers(isOpen);
            lastAppliedOpenState = isOpen;
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log(lockedDoorHint);
            if (gameManager != null && !string.IsNullOrWhiteSpace(lockedDoorHint))
            {
                gameManager.ShowHint(lockedDoorHint);
            }
            return;
        }

        isOpen = !isOpen;
    }

    public void UnlockAndOpen()
    {
        isLocked = false;
        isOpen = true;
        if (audioSource != null && openDoorSound != null)
        {
            audioSource.PlayOneShot(openDoorSound);
        }
        Debug.Log("Porta destrancada!");
    }

    private void CacheClosedState()
    {
        closedWorldPos = transform.position;
        openWorldPos = closedWorldPos + Vector3.up * openDistance;

        Vector3 direction = localSlideDirection.sqrMagnitude < 0.0001f
            ? Vector3.right
            : localSlideDirection.normalized;
        closedLocalPos = transform.localPosition;
        openLocalPos = closedLocalPos + (direction * openDistance);

        if (hingePivot == null)
        {
            hingePivot = transform;
        }

        closedHingeLocalRotation = hingePivot.localRotation;
        openHingeLocalRotation = closedHingeLocalRotation * Quaternion.Euler(0f, hingeOpenAngle, 0f);
    }

    private void ApplyPassageBlockers(bool doorIsOpen)
    {
        if (objectsToEnableWhenOpen != null)
        {
            for (int i = 0; i < objectsToEnableWhenOpen.Length; i++)
            {
                GameObject go = objectsToEnableWhenOpen[i];
                if (go != null)
                {
                    go.SetActive(doorIsOpen);
                }
            }
        }
    }
}