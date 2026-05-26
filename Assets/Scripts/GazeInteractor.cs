using UnityEngine;
using System;

public class GazeInteractor : MonoBehaviour
{
    public float interactDistance = 20f;
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("Raio do assist de foco. 0 usa raycast puro.")]
    public float focusAssistRadius = 0.03f;
    [Tooltip("Layers que podem ser interagidas pelo olhar.")]
    public LayerMask interactLayerMask = ~0;
    [Header("Reticle")]
    [SerializeField] private bool reticleEnabled = false;
    [SerializeField] private float reticleSize = 12f;
    [SerializeField] private Color reticleColor = Color.yellow;

    private Transform currentLookTarget;
    private Texture2D reticleTexture;

    public Transform CurrentLookTarget => currentLookTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Transform hitTarget = GetBestTarget(ray);

        if (hitTarget != null)
        {
            if (currentLookTarget != hitTarget)
            {
                if (currentLookTarget != null)
                {
                    currentLookTarget.SendMessageUpwards("OnLookExit", SendMessageOptions.DontRequireReceiver);
                }

                currentLookTarget = hitTarget;
                currentLookTarget.SendMessageUpwards("OnLookEnter", SendMessageOptions.DontRequireReceiver);
            }

            if (GameInput.GetInteractDown(interactKey))
            {
                hitTarget.SendMessageUpwards("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (currentLookTarget != null)
            {
                currentLookTarget.SendMessageUpwards("OnLookExit", SendMessageOptions.DontRequireReceiver);
                currentLookTarget = null;
            }
        }
    }

    private Transform GetBestTarget(Ray ray)
    {
        if (focusAssistRadius <= 0f)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayerMask))
            {
                return hit.collider.transform;
            }

            return null;
        }

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            focusAssistRadius,
            interactDistance,
            interactLayerMask
        );

        if (hits == null || hits.Length == 0) return null;

        int bestIndex = -1;
        float bestScore = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform t = hits[i].collider.transform;
            int priority = HasInteractMethodInParents(t) ? 0 : 1;
            float score = priority * 1000f + hits[i].distance;

            if (score < bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex >= 0 ? hits[bestIndex].collider.transform : null;
    }

    private bool HasInteractMethodInParents(Transform t)
    {
        Transform current = t;
        while (current != null)
        {
            MonoBehaviour[] components = current.GetComponents<MonoBehaviour>();
            for (int i = 0; i < components.Length; i++)
            {
                MonoBehaviour mb = components[i];
                if (mb == null) continue;
                Type type = mb.GetType();
                if (type.GetMethod("Interact", Type.EmptyTypes) != null)
                {
                    return true;
                }
            }

            current = current.parent;
        }

        return false;
    }

    public void SetReticleEnabled(bool enabledState)
    {
        reticleEnabled = enabledState;
    }

    private void OnGUI()
    {
        if (!reticleEnabled) return;

        if (reticleTexture == null)
        {
            reticleTexture = new Texture2D(1, 1);
            reticleTexture.SetPixel(0, 0, Color.white);
            reticleTexture.Apply();
        }

        Color previousColor = GUI.color;
        GUI.color = reticleColor;

        float half = reticleSize * 0.5f;
        Rect rect = new Rect(
            (Screen.width * 0.5f) - half,
            (Screen.height * 0.5f) - half,
            reticleSize,
            reticleSize
        );
        GUI.DrawTexture(rect, reticleTexture);

        GUI.color = previousColor;
    }
}