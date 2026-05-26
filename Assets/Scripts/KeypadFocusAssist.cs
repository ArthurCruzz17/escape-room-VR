using UnityEngine;

public class KeypadFocusAssist : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GazeInteractor gazeInteractor;
    [SerializeField] private Transform playerView;

    [Header("Assist Settings")]
    [SerializeField] private float activationDistance = 2.2f;

    private bool assistEnabled;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        ResolveReferences();
        if (gazeInteractor == null || playerView == null) return;

        bool isNear = Vector3.Distance(playerView.position, transform.position) <= activationDistance;
        if (isNear != assistEnabled)
        {
            SetAssist(isNear);
        }
    }

    private void SetAssist(bool enabledState)
    {
        assistEnabled = enabledState;
        gazeInteractor.SetReticleEnabled(assistEnabled);
    }

    private void ResolveReferences()
    {
        if (gazeInteractor == null)
        {
            gazeInteractor = Object.FindFirstObjectByType<GazeInteractor>();
        }

        if (playerView == null && Camera.main != null)
        {
            playerView = Camera.main.transform;
        }
    }

    private void OnDisable()
    {
        if (gazeInteractor != null)
        {
            gazeInteractor.SetReticleEnabled(false);
        }

        assistEnabled = false;
    }
}
