using NavKeypad;
using UnityEngine;

public class KeypadGameBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Keypad keypad;
    [SerializeField] private DoorController door;
    [SerializeField] private GameManager gameManager;

    [Header("Optional Feedback")]
    [TextArea]
    [SerializeField] private string wrongCodeHint = "Codigo incorreto!";

    private void Awake()
    {
        if (keypad == null) keypad = GetComponent<Keypad>();
        if (door == null) door = Object.FindFirstObjectByType<DoorController>();
        if (gameManager == null) gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        if (keypad == null)
        {
            Debug.LogWarning("KeypadGameBridge: Keypad nao atribuido.");
            return;
        }

        keypad.OnAccessGranted.RemoveListener(HandleAccessGranted);
        keypad.OnAccessGranted.AddListener(HandleAccessGranted);

        keypad.OnAccessDenied.RemoveListener(HandleAccessDenied);
        keypad.OnAccessDenied.AddListener(HandleAccessDenied);
    }

    private void OnDisable()
    {
        if (keypad == null) return;
        keypad.OnAccessGranted.RemoveListener(HandleAccessGranted);
        keypad.OnAccessDenied.RemoveListener(HandleAccessDenied);
    }

    private void HandleAccessGranted()
    {
        if (door != null)
        {
            door.UnlockAndOpen();
        }
        else
        {
            Debug.LogWarning("KeypadGameBridge: DoorController nao encontrado.");
        }

        if (gameManager != null && !gameManager.IsGameEnded())
        {
            gameManager.WinGame();
        }
    }

    private void HandleAccessDenied()
    {
        if (gameManager != null && !string.IsNullOrWhiteSpace(wrongCodeHint))
        {
            gameManager.ShowHint(wrongCodeHint);
        }
    }
}
