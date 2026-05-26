using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NavKeypad
{
    public class Keypad : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted;
        [SerializeField] private UnityEvent onAccessDenied;
        [Header("Combination Code (9 Numbers Max)")]
        [SerializeField] private int keypadCombo = 12345;

        public UnityEvent OnAccessGranted => onAccessGranted;
        public UnityEvent OnAccessDenied => onAccessDenied;

        [Header("Settings")]
        [SerializeField] private string accessGrantedText = "Granted";
        [SerializeField] private string accessDeniedText = "Denied";

        [Header("Visuals")]
        [SerializeField] private float displayResultTime = 1f;
        [Range(0, 5)]
        [SerializeField] private float screenIntensity = 2.5f;
        [Header("Colors")]
        [SerializeField] private Color screenNormalColor = new Color(0.98f, 0.50f, 0.032f, 1f);
        [SerializeField] private Color screenDeniedColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private Color screenGrantedColor = new Color(0f, 0.62f, 0.07f);
        [Header("SoundFx")]
        [SerializeField] private AudioClip buttonClickedSfx;
        [SerializeField] private AudioClip accessDeniedSfx;
        [SerializeField] private AudioClip accessGrantedSfx;
        [Header("Component References")]
        [SerializeField] private Renderer panelMesh;
        [SerializeField] private TMP_Text keypadDisplayText;
        [SerializeField] private AudioSource audioSource;


        private string currentInput;
        private bool displayingResult = false;
        private bool accessWasGranted = false;

        private void Awake()
        {
            ClearInput();
            if (panelMesh != null)
            {
                panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
            }
        }


        public void AddInput(string input)
        {
            if (displayingResult || accessWasGranted) return;
            if (audioSource != null && buttonClickedSfx != null)
            {
                audioSource.PlayOneShot(buttonClickedSfx);
            }

            switch (input)
            {
                case "enter":
                    CheckCombo();
                    break;
                default:
                    if (currentInput != null && currentInput.Length == 9)
                    {
                        return;
                    }
                    currentInput += input;
                    if (keypadDisplayText != null)
                    {
                        keypadDisplayText.text = currentInput;
                    }
                    break;
            }

        }
        public void CheckCombo()
        {
            if (int.TryParse(currentInput, out var currentKombo))
            {
                bool granted = currentKombo == keypadCombo;
                if (!displayingResult)
                {
                    StartCoroutine(DisplayResultRoutine(granted));
                }
            }
            else
            {
                Debug.LogWarning("Couldn't process input for some reason..");
            }

        }

        private IEnumerator DisplayResultRoutine(bool granted)
        {
            displayingResult = true;

            if (granted) AccessGranted();
            else AccessDenied();

            yield return new WaitForSeconds(displayResultTime);
            displayingResult = false;
            if (granted) yield break;
            ClearInput();
            if (panelMesh != null)
            {
                panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
            }

        }

        private void AccessDenied()
        {
            if (keypadDisplayText != null)
            {
                keypadDisplayText.text = accessDeniedText;
            }
            onAccessDenied?.Invoke();
            if (panelMesh != null)
            {
                panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
            }

            if (audioSource != null && accessDeniedSfx != null)
            {
                audioSource.PlayOneShot(accessDeniedSfx);
            }
        }

        private void ClearInput()
        {
            currentInput = "";
            if (keypadDisplayText != null)
            {
                keypadDisplayText.text = currentInput;
            }
        }

        private void AccessGranted()
        {
            accessWasGranted = true;
            if (keypadDisplayText != null)
            {
                keypadDisplayText.text = accessGrantedText;
            }
            onAccessGranted?.Invoke();
            if (panelMesh != null)
            {
                panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            }

            if (audioSource != null && accessGrantedSfx != null)
            {
                audioSource.PlayOneShot(accessGrantedSfx);
            }
        }

    }
}