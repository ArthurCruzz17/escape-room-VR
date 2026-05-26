using UnityEngine;

public class LookHighlight : MonoBehaviour
{
    public Color highlightEmissionColor = new Color(0.35f, 0.35f, 0.0f);

    private Renderer[] renderers;
    private Color[] originalEmissionColors;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalEmissionColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material;
            mat.EnableKeyword("_EMISSION");
            originalEmissionColors[i] = mat.GetColor("_EmissionColor");
        }
    }

    public void OnLookEnter()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetColor("_EmissionColor", highlightEmissionColor);
        }
    }

    public void OnLookExit()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetColor("_EmissionColor", originalEmissionColors[i]);
        }
    }
}