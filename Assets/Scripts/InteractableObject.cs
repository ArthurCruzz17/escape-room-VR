using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [TextArea]
    public string hintMessage;
    [SerializeField] private float hintDuration = 3.5f;

    public void Interact()
    {
        Debug.Log("Dica: " + hintMessage);

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ShowHint(hintMessage, hintDuration);
        }
    }
}