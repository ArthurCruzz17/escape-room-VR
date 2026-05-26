using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System.Text;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class GameManager : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;
    public GameObject winText;
    public GameObject loseText;
    public TMP_Text hintText;

    [Header("Timer Readability")]
    [SerializeField] private bool applyTimerStyleOnStart = true;
    [SerializeField] private Color timerFaceColor = Color.white;
    [SerializeField] private Color timerOutlineColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float timerOutlineWidth = 0.2f;

    [Header("Timer Urgency Colors")]
    [SerializeField] private int warningSeconds = 10;
    [SerializeField] private int dangerSeconds = 5;
    [SerializeField] private Color warningTimerColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color dangerTimerColor = new Color(1f, 0.3f, 0.3f);

    [Header("Hint Text")]
    [SerializeField] private bool applyHintStyleOnStart = true;
    [SerializeField] private Color hintFaceColor = Color.white;
    [SerializeField] private Color hintOutlineColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float hintOutlineWidth = 0.25f;
    [SerializeField] private float defaultHintDuration = 3.5f;
    [SerializeField] private bool hideHintWhenEmpty = true;
    [SerializeField] private bool forceHintBottomLeft = true;
    [SerializeField] private Vector2 hintBottomLeftMargin = new Vector2(24f, 24f);
    [SerializeField] private float hintLineHeight = 42f;
    [SerializeField] private float hintLineGap = 8f;
    [SerializeField] private int hintLineIndex = 0;
    [SerializeField] private int timerLineIndex = 1;
    [SerializeField] private string loseHintMessage = "Tempo esgotado! Pressione R para tentar novamente.";
    [SerializeField] private float loseHintDuration = 12f;

    [Header("Win Sequence")]
    [SerializeField] private bool fadeToBlackOnWin = true;
    [SerializeField] private float winFadeDelay = 0.25f;
    [SerializeField] private float winFadeDuration = 0.6f;
    [SerializeField] private Color winFadeColor = Color.black;

    [Header("Win Music")]
    [SerializeField] private AudioSource winMusicSource;
    [SerializeField] private AudioClip winMusicClip;
    [SerializeField, Min(0f)] private float winMusicDelayFromLogoAppearance = 0f;
    [SerializeField, Min(0f)] private float winMusicStartTime = 0f;
    [SerializeField, Min(0.1f)] private float winMusicPreviewDuration = 30f;
    [SerializeField, Min(0f)] private float winMusicFadeOutDuration = 1.5f;

    [Header("Win Credits")]
    [SerializeField] private TMP_Text winCreditsText;
    [TextArea(4, 12)]
    [SerializeField] private string winCreditsContent =
        "EPISODIO FINAL\nCODEBREAKER CONCLUIDO\n\n" +
        "Obrigado por jogar!\n\n" +
        "Direcao e Design\nArthur Cruz\n\n" +
        "Programacao\nArthur Cruz\n\n" +
        "Ate a proxima sala...";
    [SerializeField, Min(1f)] private float winCreditsScrollDuration = 16f;
    [SerializeField] private float winCreditsStartY = 520f;
    [SerializeField] private float winCreditsEndY = -420f;
    [SerializeField] private bool hideGameplayHudOnWin = true;
    [SerializeField] private AnimationCurve winCreditsProgressCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve winCreditsAlphaCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(0.82f, 1f),
        new Keyframe(1f, 0.05f));
    [SerializeField] private bool enableCrawlHorizonFade = true;
    [SerializeField, Range(0.15f, 0.75f)] private float crawlHorizonFadeHeight = 0.42f;
    [SerializeField] private Image winStarfieldImage;
    [SerializeField] private Sprite winStarfieldSprite;
    [SerializeField] private Color winStarfieldTint = new Color(1f, 1f, 1f, 0.2f);

    [Header("Star Wars Credits Style")]
    [SerializeField] private bool useStarWarsCreditsStyle = true;
    [SerializeField] private Color starWarsCreditsColor = new Color(1f, 0.86f, 0.2f);
    [SerializeField] private float starWarsStartScale = 1.1f;
    [SerializeField] private float starWarsEndScale = 0.42f;
    [SerializeField] private Vector3 starWarsTiltEuler = new Vector3(26f, 0f, 0f);

    [Header("Star Wars Intro Title")]
    [SerializeField] private TMP_Text winIntroTitleText;
    [SerializeField] private Image winIntroTitleImage;
    [SerializeField] private Sprite winIntroTitleSprite;
    [SerializeField] private string winIntroTitleContent = "ESCAPE ROOM VR: CODEBREAKER";
    [SerializeField] private TMP_FontAsset starWarsIntroTitleFont;
    [SerializeField] private Color winIntroTitleColor = new Color(1f, 0.86f, 0.2f);
    [SerializeField, Min(0.2f)] private float winIntroTitleZoomDuration = 11.7f;
    [SerializeField, Min(1f)] private float winIntroTitleFontSize = 220f;
    [SerializeField] private Vector2 winIntroTitleImageSize = new Vector2(2000f, 700f);
    [SerializeField] private float winIntroTitleStartScale = 3.4f;
    [SerializeField] private float winIntroTitleEndScale = 0.12f;
    [SerializeField] private float winIntroTitleStartY = -60f;
    [SerializeField] private float winIntroTitleEndY = 150f;
    [SerializeField, Range(0f, 0.95f)] private float winIntroTitleFadeStartNormalized = 0.28f;
    [SerializeField, Min(0f)] private float winCreditsLeadInDuringIntro = 5f;
    [SerializeField] private bool applyIntroLogoGlow = true;
    [SerializeField] private Color introLogoGlowColor = new Color(1f, 0.8f, 0.25f, 0.55f);
    [SerializeField] private Vector2 introLogoGlowDistance = new Vector2(8f, -8f);

    [Header("End Restart Prompt")]
    [SerializeField] private TMP_Text restartPromptText;
    [SerializeField] private string restartPromptContent = "Pressione R ou Start para reiniciar o jogo";
    [SerializeField, Min(10f)] private float restartPromptFontSize = 34f;
    [SerializeField, Min(0f)] private float restartPromptLeadTimeBeforeCreditsEnd = 5f;
    [SerializeField] private bool restartPromptBlink = true;
    [SerializeField, Min(0.1f)] private float restartPromptBlinkSpeed = 2.6f;
    [SerializeField, Range(0f, 1f)] private float restartPromptMinAlpha = 0.35f;

    [Header("Interaction UI")]
    [SerializeField] private GazeInteractor gazeInteractor;
    [SerializeField] private PlayerController playerController;

    [Header("Start Screen")]
    [SerializeField] private bool requireManualStart = true;
    [SerializeField] private string startScreenTitle = "ESCAPE ROOM VR";
    [SerializeField] private string startScreenSubtitle = "CODEBREAKER";
    [SerializeField] private string startScreenObjective = "Encontre 3 pistas, descubra a senha e escape antes do tempo acabar.";
    [SerializeField] private string startScreenKeyboardMouseControls = "TECLADO E MOUSE: Mover: WASD | Olhar: Mouse | Interagir: E";
    [SerializeField] private string startScreenGamepadControls = "CONTROLE: Mover: Analogico esquerdo | Olhar: Analogico direito | Interagir: A";
    [SerializeField] private string startScreenPrompt = "Pressione E no teclado ou Start no controle para comecar o jogo!";
    [SerializeField] private Color startScreenBackgroundColor = new Color(0f, 0f, 0f, 0.88f);
    [SerializeField] private Color startScreenTitleColor = new Color(1f, 1f, 1f, 0.98f);
    [SerializeField] private Color startScreenPromptColor = Color.white;
    [SerializeField] private Color startScreenPanelColor = new Color(0.07f, 0.06f, 0.05f, 0.9f);
    [SerializeField] private Color startScreenPanelBorderColor = new Color(1f, 1f, 1f, 0.18f);
    [SerializeField] private Color startScreenSectionColor = new Color(1f, 1f, 1f, 0.08f);
    [Header("Start Screen Typography")]
    [SerializeField] private TMP_FontAsset startScreenTitleFont;
    [SerializeField] private TMP_FontAsset startScreenBodyFont;
    [SerializeField] private FontWeight startScreenTitleWeight = FontWeight.Black;
    [SerializeField, Range(-10f, 25f)] private float startScreenTitleCharacterSpacing = 7f;
    [SerializeField] private FontWeight startScreenBodyWeight = FontWeight.Medium;
    [SerializeField, Range(-10f, 15f)] private float startScreenBodyCharacterSpacing = 0.5f;

    private bool gameEnded = false;
    private bool gameStarted;
    private Image winFadeOverlay;
    private Coroutine winFadeCoroutine;
    private Coroutine winMusicStartCoroutine;
    private Coroutine winMusicStopCoroutine;
    private Coroutine winCreditsCoroutine;
    private Coroutine restartPromptBlinkCoroutine;
    private bool canRestartNow;
    private bool winMusicTriggeredThisWin;
    private float winMusicBaseVolume = 1f;
    private Image crawlHorizonFadeImage;
    private Sprite crawlHorizonFadeSprite;
    private GameObject startScreenCanvasObject;
    private const string DefaultStartScreenPrompt = "Pressione E no teclado ou Start no controle para comecar o jogo!";

    void Start()
    {
        startScreenPrompt = DefaultStartScreenPrompt;
        EnsureStartScreenFonts();
        ApplyGameplayFontsToSceneTexts();

        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);

        if (applyTimerStyleOnStart && timerText != null)
        {
            timerText.fontStyle |= FontStyles.Bold;
            timerText.enableWordWrapping = false;
            timerText.overflowMode = TextOverflowModes.Overflow;
            timerText.color = timerFaceColor;
            timerText.outlineColor = timerOutlineColor;
            timerText.outlineWidth = timerOutlineWidth;
        }

        if (hintText != null)
        {
            if (applyHintStyleOnStart)
            {
                hintText.fontStyle |= FontStyles.Bold;
                hintText.color = hintFaceColor;
                hintText.outlineColor = hintOutlineColor;
                hintText.outlineWidth = hintOutlineWidth;
            }

            hintText.enableWordWrapping = false;
            hintText.overflowMode = TextOverflowModes.Overflow;
            hintText.alignment = TextAlignmentOptions.BottomLeft;

            if (forceHintBottomLeft)
            {
                ApplyBottomLeftLineLayout(hintText.rectTransform, hintLineIndex);
            }

            if (hideHintWhenEmpty)
            {
                hintText.gameObject.SetActive(false);
            }
        }

        if (fadeToBlackOnWin)
        {
            EnsureWinFadeOverlay();
        }

        if (winCreditsText != null)
        {
            winCreditsText.gameObject.SetActive(false);
            winCreditsText.enableWordWrapping = true;
            winCreditsText.enableAutoSizing = false;
            winCreditsText.alignment = TextAlignmentOptions.Top;
        }

        if (winIntroTitleText != null)
        {
            winIntroTitleText.gameObject.SetActive(false);
            winIntroTitleText.enableWordWrapping = true;
            winIntroTitleText.alignment = TextAlignmentOptions.Center;
        }
        if (restartPromptText != null)
        {
            ApplyRestartPromptTypography(restartPromptText);
            restartPromptText.fontSize = restartPromptFontSize;
            restartPromptText.gameObject.SetActive(false);
        }
        if (winIntroTitleImage != null)
        {
            winIntroTitleImage.gameObject.SetActive(false);
            if (winIntroTitleSprite != null)
            {
                winIntroTitleImage.sprite = winIntroTitleSprite;
            }
            winIntroTitleImage.preserveAspect = true;
            ApplyIntroLogoGlow(winIntroTitleImage);
        }
        if (winStarfieldImage != null)
        {
            winStarfieldImage.gameObject.SetActive(false);
            if (winStarfieldSprite != null)
            {
                winStarfieldImage.sprite = winStarfieldSprite;
            }
            winStarfieldImage.color = winStarfieldTint;
        }

        if (gazeInteractor == null)
        {
            gazeInteractor = Object.FindFirstObjectByType<GazeInteractor>();
        }

        if (playerController == null)
        {
            playerController = Object.FindFirstObjectByType<PlayerController>();
        }

        gameStarted = !requireManualStart;
        if (requireManualStart)
        {
            ShowStartScreen();
        }
        else
        {
            EnableGameplayControls(true);
        }

        RefreshTimerText();
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (ShouldStartGameNow())
            {
                BeginGame();
            }
            return;
        }

        if (canRestartNow && GameInput.GetRestartDown(KeyCode.R))
        {
            RestartGame();
            return;
        }
        if (gameEnded) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            LoseGame();
        }

        if (timerText != null)
        {
            RefreshTimerText();
        }
    }

    private bool ShouldStartGameNow()
    {
        return GameInput.GetStartGameDown(KeyCode.E);
    }

    public void BeginGame()
    {
        if (gameStarted || gameEnded) return;

        gameStarted = true;
        if (startScreenCanvasObject != null)
        {
            startScreenCanvasObject.SetActive(false);
        }
        FinalizeGameStart();
    }

    private void ShowStartScreen()
    {
        EnableGameplayControls(false);
        EnsureStartScreenCanvas();

        if (startScreenCanvasObject != null)
        {
            startScreenCanvasObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnableGameplayControls(bool enabled)
    {
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        if (gazeInteractor != null)
        {
            gazeInteractor.enabled = enabled;
            if (!enabled)
            {
                gazeInteractor.SetReticleEnabled(false);
            }
        }
    }

    private void RefreshTimerText()
    {
        if (timerText == null) return;
        timerText.text = "Tempo: " + Mathf.CeilToInt(timeRemaining).ToString();
        timerText.color = GetTimerColorByRemainingTime(timeRemaining);
    }

    private void EnsureStartScreenCanvas()
    {
        if (startScreenCanvasObject != null) return;
        EnsureStartScreenFonts();

        startScreenCanvasObject = new GameObject("StartScreenCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = startScreenCanvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = startScreenCanvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject backgroundObject = new GameObject("Background", typeof(RectTransform), typeof(Image));
        backgroundObject.transform.SetParent(startScreenCanvasObject.transform, false);
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        backgroundObject.GetComponent<Image>().color = startScreenBackgroundColor;

        GameObject panelObject = new GameObject("MenuPanel", typeof(RectTransform), typeof(Image), typeof(Outline));
        panelObject.transform.SetParent(startScreenCanvasObject.transform, false);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1260f, 780f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = startScreenPanelColor;

        Outline panelOutline = panelObject.GetComponent<Outline>();
        panelOutline.effectColor = startScreenPanelBorderColor;
        panelOutline.effectDistance = new Vector2(2f, -2f);

        CreateStartScreenLabel(panelObject.transform, "Title", startScreenTitle, 82f, startScreenTitleColor, new Vector2(0f, 285f), FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(1100f, 108f), startScreenTitleFont);
        CreateStartScreenLabel(panelObject.transform, "Subtitle", startScreenSubtitle, 42f, startScreenTitleColor, new Vector2(0f, 225f), FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(1100f, 78f), startScreenTitleFont);

        const float objectiveBlockY = 120f;
        const float startBlockY = -165f;
        float controlsBlockY = (objectiveBlockY + startBlockY) * 0.5f;

        CreateStartScreenInfoBlock(panelObject.transform, "ObjectiveBlock", "OBJETIVO", startScreenObjective, new Vector2(0f, objectiveBlockY), 92f, 22f, -12f);
        CreateStartScreenInfoBlock(panelObject.transform, "ControlsBlock", "CONTROLES", $"{startScreenKeyboardMouseControls}\n{startScreenGamepadControls}", new Vector2(0f, controlsBlockY), 168f, 48f, -30f);
        CreateStartScreenInfoBlock(panelObject.transform, "StartBlock", "INICIAR", startScreenPrompt, new Vector2(0f, startBlockY), 92f, 22f, -12f);
    }

    private TextMeshProUGUI CreateStartScreenInfoBlock(Transform parent, string objectName, string header, string body, Vector2 anchoredPosition, float height, float headerY = 30f, float bodyY = -12f, bool usePromptColorForHeader = false)
    {
        GameObject blockObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Outline));
        blockObject.transform.SetParent(parent, false);

        RectTransform blockRect = blockObject.GetComponent<RectTransform>();
        blockRect.anchorMin = new Vector2(0.5f, 0.5f);
        blockRect.anchorMax = new Vector2(0.5f, 0.5f);
        blockRect.pivot = new Vector2(0.5f, 0.5f);
        blockRect.sizeDelta = new Vector2(1080f, height);
        blockRect.anchoredPosition = anchoredPosition;

        Image blockImage = blockObject.GetComponent<Image>();
        blockImage.color = startScreenSectionColor;

        Outline blockOutline = blockObject.GetComponent<Outline>();
        blockOutline.effectColor = new Color(1f, 1f, 1f, 0.16f);
        blockOutline.effectDistance = new Vector2(1f, -1f);

        Color headerColor = usePromptColorForHeader ? startScreenPromptColor : startScreenTitleColor;
        CreateStartScreenLabel(blockObject.transform, "Header", header, 28f, headerColor, new Vector2(0f, headerY), FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(980f, 40f), startScreenTitleFont);
        return CreateStartScreenLabel(blockObject.transform, "Body", body, 25f, startScreenPromptColor, new Vector2(0f, bodyY), FontStyles.Normal, TextAlignmentOptions.Center, new Vector2(980f, 76f), startScreenBodyFont);
    }

    private TextMeshProUGUI CreateStartScreenLabel(Transform parent, string objectName, string content, float fontSize, Color color, Vector2 anchoredPosition, FontStyles style, TextAlignmentOptions alignment, Vector2 size, TMP_FontAsset customFont)
    {
        if (parent == null) return null;

        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        bool isLeftAligned = alignment == TextAlignmentOptions.Left ||
                             alignment == TextAlignmentOptions.TopLeft ||
                             alignment == TextAlignmentOptions.BottomLeft ||
                             alignment == TextAlignmentOptions.MidlineLeft ||
                             alignment == TextAlignmentOptions.BaselineLeft;

        if (isLeftAligned)
        {
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;
        if (customFont != null)
        {
            text.font = customFont;
        }

        if (customFont == startScreenTitleFont)
        {
            text.fontWeight = startScreenTitleWeight;
            text.characterSpacing = startScreenTitleCharacterSpacing;
        }
        else
        {
            text.fontWeight = startScreenBodyWeight;
            text.characterSpacing = startScreenBodyCharacterSpacing;
        }
        return text;
    }

    private void EnsureStartScreenFonts()
    {
        if (startScreenTitleFont == null && starWarsIntroTitleFont != null)
        {
            startScreenTitleFont = starWarsIntroTitleFont;
        }

        if (startScreenBodyFont == null)
        {
            startScreenBodyFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (startScreenBodyFont == null)
            {
                startScreenBodyFont = TMP_Settings.defaultFontAsset;
            }
        }

        if (startScreenTitleFont == null)
        {
            startScreenTitleFont = startScreenBodyFont;
        }
    }

    private void ApplyGameplayFontsToSceneTexts()
    {
        TMP_Text[] sceneTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < sceneTexts.Length; i++)
        {
            TMP_Text text = sceneTexts[i];
            if (text == null) continue;

            if (timerText != null && text == timerText)
            {
                ApplyTimerTypography(text);
            }
            else if ((winCreditsText != null && text == winCreditsText) ||
                     (restartPromptText != null && text == restartPromptText))
            {
                continue;
            }
            else
            {
                ApplyBodyTypography(text);
            }
        }
    }

    private void ApplyTimerTypography(TMP_Text text)
    {
        if (text == null || startScreenTitleFont == null) return;
        text.font = startScreenTitleFont;
        text.fontWeight = FontWeight.Bold;
        text.characterSpacing = 0f;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
    }

    private void ApplyBodyTypography(TMP_Text text)
    {
        if (text == null || startScreenBodyFont == null) return;
        text.font = startScreenBodyFont;
        text.fontWeight = startScreenBodyWeight;
        text.characterSpacing = startScreenBodyCharacterSpacing;
    }

    private static void ApplyRestartPromptTypography(TMP_Text text)
    {
        if (text == null) return;
        if (TMP_Settings.defaultFontAsset != null)
        {
            text.font = TMP_Settings.defaultFontAsset;
        }
        text.fontWeight = FontWeight.Bold;
        text.characterSpacing = 0f;
    }

    private void FinalizeGameStart()
    {
        EnableGameplayControls(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private Color GetTimerColorByRemainingTime(float remaining)
    {
        if (remaining <= dangerSeconds)
        {
            return dangerTimerColor;
        }

        if (remaining <= warningSeconds)
        {
            return warningTimerColor;
        }

        return timerFaceColor;
    }

    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        canRestartNow = false;
        winMusicTriggeredThisWin = false;

        if (winText != null) winText.SetActive(false);
        HideRestartPrompt();
        HideInteractionReticle();

        if (fadeToBlackOnWin)
        {
            if (winFadeCoroutine != null)
            {
                StopCoroutine(winFadeCoroutine);
            }

            winFadeCoroutine = StartCoroutine(WinFadeThenStartCreditsRoutine());
        }
        else
        {
            StartCoroutine(WinIntroThenStartCreditsRoutine());
        }

        if (hideGameplayHudOnWin)
        {
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (hintText != null) hintText.gameObject.SetActive(false);
        }
        Debug.Log("Você escapou!");
    }

    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        canRestartNow = true;
        HideInteractionReticle();

        if (loseText != null) loseText.SetActive(false);
        ShowHint(loseHintMessage, loseHintDuration);
        Debug.Log("Tempo esgotado!");
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
    public void RestartGame()
    {
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );
    }
    public void ShowHint(string message)
    {
        ShowHint(message, defaultHintDuration);
    }

    public void ShowHint(string message, float duration)
    {
        if (hintText == null) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        string singleLineMessage = message.Replace('\n', ' ').Replace('\r', ' ');
        hintText.text = singleLineMessage;
        if (hideHintWhenEmpty)
        {
            hintText.gameObject.SetActive(true);
        }

        CancelInvoke(nameof(ClearHint));
        Invoke(nameof(ClearHint), Mathf.Max(0.1f, duration));
    }

    void ClearHint()
    {
        if (hintText == null) return;

        hintText.text = "";
        if (hideHintWhenEmpty)
        {
            hintText.gameObject.SetActive(false);
        }
    }

    private void EnsureWinFadeOverlay()
    {
        if (winFadeOverlay != null) return;

        GameObject canvasObj = new GameObject("WinFadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 900;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("WinFadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        winFadeOverlay = imageObj.AddComponent<Image>();
        winFadeOverlay.raycastTarget = false;
        Color c = winFadeColor;
        c.a = 0f;
        winFadeOverlay.color = c;
    }

    private System.Collections.IEnumerator WinFadeRoutine()
    {
        EnsureWinFadeOverlay();
        if (winFadeOverlay == null) yield break;

        yield return new WaitForSeconds(winFadeDelay);

        float elapsed = 0f;
        while (elapsed < winFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, winFadeDuration));
            Color c = winFadeColor;
            c.a = t;
            winFadeOverlay.color = c;
            yield return null;
        }

        Color finalColor = winFadeColor;
        finalColor.a = 1f;
        winFadeOverlay.color = finalColor;
    }

    private void PlayWinMusicPreview()
    {
        if (winMusicSource == null || winMusicClip == null)
        {
            return;
        }

        winMusicSource.Stop();
        winMusicSource.clip = winMusicClip;
        winMusicBaseVolume = winMusicSource.volume;
        winMusicSource.volume = winMusicBaseVolume;
        winMusicSource.time = Mathf.Clamp(winMusicStartTime, 0f, Mathf.Max(0f, winMusicClip.length - 0.01f));
        winMusicSource.Play();

        if (winMusicStopCoroutine != null)
        {
            StopCoroutine(winMusicStopCoroutine);
        }

        float previewDuration = Mathf.Max(30f, winMusicPreviewDuration);
        winMusicStopCoroutine = StartCoroutine(StopWinMusicAfterPreview(previewDuration));
    }

    private void ScheduleWinMusicPreview()
    {
        if (winMusicTriggeredThisWin) return;
        winMusicTriggeredThisWin = true;

        if (winMusicSource != null && winMusicSource.isPlaying)
        {
            winMusicSource.Stop();
        }

        if (winMusicStartCoroutine != null)
        {
            StopCoroutine(winMusicStartCoroutine);
        }

        winMusicStartCoroutine = StartCoroutine(PlayWinMusicPreviewDelayedRoutine());
    }

    private System.Collections.IEnumerator PlayWinMusicPreviewDelayedRoutine()
    {
        if (winMusicDelayFromLogoAppearance > 0f)
        {
            yield return new WaitForSeconds(winMusicDelayFromLogoAppearance);
        }

        PlayWinMusicPreview();
        winMusicStartCoroutine = null;
    }

    private System.Collections.IEnumerator StopWinMusicAfterPreview(float previewDuration)
    {
        if (winMusicSource == null)
        {
            winMusicStopCoroutine = null;
            yield break;
        }

        float fadeDuration = Mathf.Clamp(winMusicFadeOutDuration, 0f, previewDuration);
        float holdDuration = Mathf.Max(0f, previewDuration - fadeDuration);
        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        if (fadeDuration > 0f && winMusicSource.isPlaying)
        {
            float elapsed = 0f;
            float startVolume = winMusicBaseVolume;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, fadeDuration));
                winMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
        }

        if (winMusicSource != null && winMusicSource.isPlaying)
        {
            winMusicSource.Stop();
        }

        if (winMusicSource != null)
        {
            winMusicSource.volume = winMusicBaseVolume;
        }

        winMusicStopCoroutine = null;
    }

    private bool StartWinCredits()
    {
        EnsureWinCreditsText();
        if (winCreditsText == null) return false;

        EnsureCreditsTextAboveBlackOverlay();
        ShowCrawlHorizonFade();

        if (winCreditsCoroutine != null)
        {
            StopCoroutine(winCreditsCoroutine);
        }

        winCreditsCoroutine = StartCoroutine(WinCreditsRoutine());
        return true;
    }

    private void EnsureWinCreditsText()
    {
        if (winCreditsText != null) return;

        GameObject creditsCanvasObj = new GameObject("WinCreditsCanvas");
        Canvas creditsCanvas = creditsCanvasObj.AddComponent<Canvas>();
        creditsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        creditsCanvas.sortingOrder = 1002;
        creditsCanvasObj.AddComponent<CanvasScaler>();
        creditsCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject creditsTextObj = new GameObject("WinCreditsText");
        creditsTextObj.transform.SetParent(creditsCanvasObj.transform, false);

        TextMeshProUGUI runtimeCreditsText = creditsTextObj.AddComponent<TextMeshProUGUI>();
        runtimeCreditsText.fontSize = 48f;
        runtimeCreditsText.fontStyle |= FontStyles.Bold;
        runtimeCreditsText.color = Color.white;
        runtimeCreditsText.outlineColor = Color.black;
        runtimeCreditsText.outlineWidth = 0.25f;
        runtimeCreditsText.enableWordWrapping = true;
        runtimeCreditsText.enableAutoSizing = false;
        runtimeCreditsText.alignment = TextAlignmentOptions.Top;

        RectTransform rect = runtimeCreditsText.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(1500f, 900f);

        runtimeCreditsText.gameObject.SetActive(false);
        winCreditsText = runtimeCreditsText;
    }

    private void ShowWinBlackOverlayImmediate()
    {
        EnsureWinFadeOverlay();
        if (winFadeOverlay == null) return;

        if (winFadeCoroutine != null)
        {
            StopCoroutine(winFadeCoroutine);
            winFadeCoroutine = null;
        }

        Color c = winFadeColor;
        c.a = 1f;
        winFadeOverlay.color = c;
    }

    private System.Collections.IEnumerator WinFadeThenStartCreditsRoutine()
    {
        EnsureWinFadeOverlay();
        if (winFadeOverlay == null)
        {
            yield return StartCoroutine(WinIntroThenStartCreditsRoutine());
            yield break;
        }

        Color c = winFadeColor;
        c.a = 0f;
        winFadeOverlay.color = c;

        yield return StartCoroutine(WinFadeRoutine());
        winFadeCoroutine = null;
        yield return StartCoroutine(WinIntroThenStartCreditsRoutine());
    }

    private void EnsureCreditsTextAboveBlackOverlay()
    {
        if (winCreditsText == null) return;

        Canvas creditsCanvas = winCreditsText.canvas;
        if (creditsCanvas == null) return;

        creditsCanvas.overrideSorting = true;
        creditsCanvas.sortingOrder = Mathf.Max(creditsCanvas.sortingOrder, 1002);
    }

    private void ShowWinStarfield()
    {
        if (winStarfieldSprite == null && winStarfieldImage == null) return;

        EnsureWinStarfieldImage();
        if (winStarfieldImage == null) return;

        if (winStarfieldSprite != null)
        {
            winStarfieldImage.sprite = winStarfieldSprite;
        }

        if (winStarfieldImage.sprite == null)
        {
            return;
        }

        winStarfieldImage.color = winStarfieldTint;
        winStarfieldImage.gameObject.SetActive(true);
    }

    private void EnsureWinStarfieldImage()
    {
        if (winStarfieldImage != null) return;

        GameObject starfieldCanvasObj = new GameObject("WinStarfieldCanvas");
        Canvas starfieldCanvas = starfieldCanvasObj.AddComponent<Canvas>();
        starfieldCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        starfieldCanvas.sortingOrder = 1001;
        starfieldCanvasObj.AddComponent<CanvasScaler>();
        starfieldCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject starfieldImageObj = new GameObject("WinStarfieldImage");
        starfieldImageObj.transform.SetParent(starfieldCanvasObj.transform, false);

        Image runtimeStarfieldImage = starfieldImageObj.AddComponent<Image>();
        runtimeStarfieldImage.raycastTarget = false;
        runtimeStarfieldImage.preserveAspect = false;
        runtimeStarfieldImage.sprite = winStarfieldSprite;
        runtimeStarfieldImage.color = winStarfieldTint;

        RectTransform rect = runtimeStarfieldImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        runtimeStarfieldImage.gameObject.SetActive(false);
        winStarfieldImage = runtimeStarfieldImage;
    }

    private void ShowCrawlHorizonFade()
    {
        if (!enableCrawlHorizonFade)
        {
            if (crawlHorizonFadeImage != null)
            {
                crawlHorizonFadeImage.gameObject.SetActive(false);
            }
            return;
        }

        EnsureCrawlHorizonFadeImage();
        if (crawlHorizonFadeImage == null) return;
        crawlHorizonFadeImage.gameObject.SetActive(true);
    }

    private void EnsureCrawlHorizonFadeImage()
    {
        if (crawlHorizonFadeImage != null) return;

        GameObject fadeCanvasObj = new GameObject("WinCrawlFadeCanvas");
        Canvas fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 1004;
        fadeCanvasObj.AddComponent<CanvasScaler>();
        fadeCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject fadeImageObj = new GameObject("WinCrawlFadeImage");
        fadeImageObj.transform.SetParent(fadeCanvasObj.transform, false);

        Image fadeImage = fadeImageObj.AddComponent<Image>();
        fadeImage.raycastTarget = false;
        fadeImage.sprite = GetOrCreateCrawlHorizonFadeSprite();
        fadeImage.type = Image.Type.Simple;
        fadeImage.color = Color.black;

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f - crawlHorizonFadeHeight);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeImage.gameObject.SetActive(false);
        crawlHorizonFadeImage = fadeImage;
    }

    private Sprite GetOrCreateCrawlHorizonFadeSprite()
    {
        if (crawlHorizonFadeSprite != null) return crawlHorizonFadeSprite;

        Texture2D tex = new Texture2D(1, 128, TextureFormat.RGBA32, false);
        tex.name = "WinCrawlHorizonFadeTexture";
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < 128; y++)
        {
            float t = y / 127f;
            float alpha = Mathf.SmoothStep(0f, 1f, t);
            tex.SetPixel(0, y, new Color(1f, 1f, 1f, alpha));
        }
        tex.Apply();

        crawlHorizonFadeSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 128f), new Vector2(0.5f, 0.5f), 100f);
        return crawlHorizonFadeSprite;
    }

    private void ApplyIntroLogoGlow(Image targetImage)
    {
        if (targetImage == null) return;

        Shadow glow = targetImage.GetComponent<Shadow>();
        if (!applyIntroLogoGlow)
        {
            if (glow != null)
            {
                glow.enabled = false;
            }
            return;
        }

        if (glow == null)
        {
            glow = targetImage.gameObject.AddComponent<Shadow>();
        }

        glow.effectColor = introLogoGlowColor;
        glow.effectDistance = introLogoGlowDistance;
        glow.useGraphicAlpha = true;
        glow.enabled = true;
    }

    private void EnsureWinIntroTitleText()
    {
        if (winIntroTitleText != null) return;

        GameObject introCanvasObj = new GameObject("WinIntroTitleCanvas");
        Canvas introCanvas = introCanvasObj.AddComponent<Canvas>();
        introCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        introCanvas.sortingOrder = 1003;
        introCanvasObj.AddComponent<CanvasScaler>();
        introCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject introTextObj = new GameObject("WinIntroTitleText");
        introTextObj.transform.SetParent(introCanvasObj.transform, false);

        TextMeshProUGUI runtimeIntroText = introTextObj.AddComponent<TextMeshProUGUI>();
        ApplyBodyTypography(runtimeIntroText);
        runtimeIntroText.fontSize = winIntroTitleFontSize;
        runtimeIntroText.fontStyle = FontStyles.Bold;
        runtimeIntroText.color = winIntroTitleColor;
        runtimeIntroText.outlineColor = Color.black;
        runtimeIntroText.outlineWidth = 0.18f;
        runtimeIntroText.enableWordWrapping = true;
        runtimeIntroText.alignment = TextAlignmentOptions.Center;
        RectTransform rect = runtimeIntroText.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(2000f, 700f);

        runtimeIntroText.gameObject.SetActive(false);
        winIntroTitleText = runtimeIntroText;
    }

    private System.Collections.IEnumerator WinIntroThenStartCreditsRoutine()
    {
        ShowWinStarfield();
        Coroutine introCoroutine = StartCoroutine(WinIntroTitleRoutine());

        float introTotalDuration = Mathf.Max(0f, winIntroTitleZoomDuration);
        float creditsDelay = Mathf.Max(0f, introTotalDuration - winCreditsLeadInDuringIntro);
        if (creditsDelay > 0f)
        {
            yield return new WaitForSeconds(creditsDelay);
        }

        StartWinCredits();
        yield return introCoroutine;
    }

    private System.Collections.IEnumerator WinIntroTitleRoutine()
    {
        bool shouldUseImageIntro = winIntroTitleSprite != null || (winIntroTitleImage != null && winIntroTitleImage.sprite != null);
        if (shouldUseImageIntro)
        {
            yield return StartCoroutine(WinIntroTitleImageRoutine());
            yield break;
        }

        EnsureWinIntroTitleText();
        if (winIntroTitleText == null) yield break;

        Canvas introCanvas = winIntroTitleText.canvas;
        if (introCanvas != null)
        {
            introCanvas.overrideSorting = true;
            introCanvas.sortingOrder = Mathf.Max(introCanvas.sortingOrder, 1003);
        }

        RectTransform introRect = winIntroTitleText.rectTransform;
        introRect.anchorMin = new Vector2(0.5f, 0.5f);
        introRect.anchorMax = new Vector2(0.5f, 0.5f);
        introRect.pivot = new Vector2(0.5f, 0.5f);

        Color baseColor = useStarWarsCreditsStyle ? starWarsCreditsColor : winIntroTitleColor;
        Color visibleColor = baseColor;
        visibleColor.a = 1f;

        winIntroTitleText.text = GetIntroTitleDisplayText();
        winIntroTitleText.fontSize = winIntroTitleFontSize;
        ApplyBodyTypography(winIntroTitleText);
        winIntroTitleText.fontStyle = FontStyles.Bold;
        winIntroTitleText.color = visibleColor;
        winIntroTitleText.gameObject.SetActive(true);

        Vector2 startPos = new Vector2(0f, winIntroTitleStartY);
        Vector2 endPos = new Vector2(0f, winIntroTitleEndY);
        float startScale = Mathf.Max(0.05f, winIntroTitleStartScale);
        float endScale = Mathf.Max(0.02f, winIntroTitleEndScale);

        introRect.anchoredPosition = startPos;
        introRect.localScale = Vector3.one * startScale;
        introRect.localRotation = Quaternion.identity;

        ScheduleWinMusicPreview();

        float elapsed = 0f;
        while (elapsed < winIntroTitleZoomDuration)
        {
            elapsed += Time.deltaTime;
            float tLinear = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, winIntroTitleZoomDuration));
            float t = tLinear;

            introRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            introRect.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

            Color c = visibleColor;
            c.a = GetIntroAlphaFromProgress(tLinear);
            winIntroTitleText.color = c;
            yield return null;
        }

        winIntroTitleText.gameObject.SetActive(false);
    }

    private void EnsureWinIntroTitleImage()
    {
        if (winIntroTitleImage != null) return;

        GameObject introCanvasObj = new GameObject("WinIntroImageCanvas");
        Canvas introCanvas = introCanvasObj.AddComponent<Canvas>();
        introCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        introCanvas.sortingOrder = 1003;
        introCanvasObj.AddComponent<CanvasScaler>();
        introCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject introImageObj = new GameObject("WinIntroTitleImage");
        introImageObj.transform.SetParent(introCanvasObj.transform, false);

        Image runtimeIntroImage = introImageObj.AddComponent<Image>();
        runtimeIntroImage.preserveAspect = true;
        runtimeIntroImage.color = Color.white;
        if (winIntroTitleSprite != null)
        {
            runtimeIntroImage.sprite = winIntroTitleSprite;
        }

        RectTransform rect = runtimeIntroImage.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = winIntroTitleImageSize;

        runtimeIntroImage.gameObject.SetActive(false);
        ApplyIntroLogoGlow(runtimeIntroImage);
        winIntroTitleImage = runtimeIntroImage;
    }

    private System.Collections.IEnumerator WinIntroTitleImageRoutine()
    {
        EnsureWinIntroTitleImage();
        if (winIntroTitleImage == null) yield break;

        if (winIntroTitleSprite != null)
        {
            winIntroTitleImage.sprite = winIntroTitleSprite;
        }
        if (winIntroTitleImage.sprite == null) yield break;
        ApplyIntroLogoGlow(winIntroTitleImage);

        if (winIntroTitleText != null)
        {
            winIntroTitleText.gameObject.SetActive(false);
        }

        Canvas introCanvas = winIntroTitleImage.canvas;
        if (introCanvas != null)
        {
            introCanvas.overrideSorting = true;
            introCanvas.sortingOrder = Mathf.Max(introCanvas.sortingOrder, 1003);
        }

        RectTransform introRect = winIntroTitleImage.rectTransform;
        introRect.anchorMin = new Vector2(0.5f, 0.5f);
        introRect.anchorMax = new Vector2(0.5f, 0.5f);
        introRect.pivot = new Vector2(0.5f, 0.5f);
        introRect.sizeDelta = winIntroTitleImageSize;

        Color visibleColor = Color.white;
        visibleColor.a = 1f;
        winIntroTitleImage.color = visibleColor;
        winIntroTitleImage.gameObject.SetActive(true);

        Vector2 startPos = new Vector2(0f, winIntroTitleStartY);
        Vector2 endPos = new Vector2(0f, winIntroTitleEndY);
        float startScale = Mathf.Max(0.05f, winIntroTitleStartScale);
        float endScale = Mathf.Max(0.02f, winIntroTitleEndScale);

        introRect.anchoredPosition = startPos;
        introRect.localScale = Vector3.one * startScale;
        introRect.localRotation = Quaternion.identity;

        ScheduleWinMusicPreview();

        float elapsed = 0f;
        while (elapsed < winIntroTitleZoomDuration)
        {
            elapsed += Time.deltaTime;
            float tLinear = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, winIntroTitleZoomDuration));
            float t = tLinear;

            introRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            introRect.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

            Color c = visibleColor;
            c.a = GetIntroAlphaFromProgress(tLinear);
            winIntroTitleImage.color = c;
            yield return null;
        }

        winIntroTitleImage.gameObject.SetActive(false);
    }

    private void EnsureRestartPromptText()
    {
        if (restartPromptText != null) return;

        GameObject promptCanvasObj = new GameObject("RestartPromptCanvas");
        Canvas promptCanvas = promptCanvasObj.AddComponent<Canvas>();
        promptCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        promptCanvas.sortingOrder = 1100;
        promptCanvasObj.AddComponent<CanvasScaler>();
        promptCanvasObj.AddComponent<GraphicRaycaster>();

        GameObject promptTextObj = new GameObject("RestartPromptText");
        promptTextObj.transform.SetParent(promptCanvasObj.transform, false);

        TextMeshProUGUI runtimePromptText = promptTextObj.AddComponent<TextMeshProUGUI>();
        ApplyRestartPromptTypography(runtimePromptText);
        runtimePromptText.fontSize = restartPromptFontSize;
        runtimePromptText.fontStyle |= FontStyles.Bold;
        runtimePromptText.color = Color.white;
        runtimePromptText.outlineColor = Color.black;
        runtimePromptText.outlineWidth = 0.2f;
        runtimePromptText.alignment = TextAlignmentOptions.Bottom;
        runtimePromptText.enableWordWrapping = false;

        RectTransform rect = runtimePromptText.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 56f);
        rect.sizeDelta = new Vector2(1400f, 120f);

        runtimePromptText.gameObject.SetActive(false);
        restartPromptText = runtimePromptText;
    }

    private void ShowRestartPrompt()
    {
        EnsureRestartPromptText();
        if (restartPromptText == null) return;

        ApplyRestartPromptTypography(restartPromptText);
        restartPromptText.fontSize = restartPromptFontSize;
        restartPromptText.text = restartPromptContent;
        restartPromptText.gameObject.SetActive(true);
        canRestartNow = true;

        if (restartPromptBlinkCoroutine != null)
        {
            StopCoroutine(restartPromptBlinkCoroutine);
            restartPromptBlinkCoroutine = null;
        }

        if (restartPromptBlink)
        {
            restartPromptBlinkCoroutine = StartCoroutine(RestartPromptBlinkRoutine());
        }
        else
        {
            Color c = restartPromptText.color;
            c.a = 1f;
            restartPromptText.color = c;
        }
    }

    private float EvaluateSequenceCurve(AnimationCurve curve, float t01)
    {
        float t = Mathf.Clamp01(t01);
        if (curve == null || curve.length == 0)
        {
            return t;
        }

        return Mathf.Clamp01(curve.Evaluate(t));
    }

    private float GetIntroAlphaFromProgress(float tLinear)
    {
        float t = Mathf.Clamp01(tLinear);
        float fadeStart = Mathf.Clamp01(winIntroTitleFadeStartNormalized);
        if (t <= fadeStart)
        {
            return 1f;
        }

        float fadeT = Mathf.InverseLerp(fadeStart, 1f, t);
        return Mathf.Clamp01(1f - Mathf.SmoothStep(0f, 1f, fadeT));
    }

    private void HideRestartPrompt()
    {
        if (restartPromptText != null)
        {
            restartPromptText.gameObject.SetActive(false);
        }

        if (restartPromptBlinkCoroutine != null)
        {
            StopCoroutine(restartPromptBlinkCoroutine);
            restartPromptBlinkCoroutine = null;
        }
    }

    private void HideInteractionReticle()
    {
        if (gazeInteractor == null)
        {
            gazeInteractor = Object.FindFirstObjectByType<GazeInteractor>();
        }

        if (gazeInteractor != null)
        {
            gazeInteractor.SetReticleEnabled(false);
        }
    }

    private System.Collections.IEnumerator WinCreditsRoutine()
    {
        if (winCreditsText == null) yield break;

        winCreditsText.text = GetCreditsContentWithoutDuplicatedTitle();
        winCreditsText.maxVisibleLines = int.MaxValue;

        RectTransform creditsRect = winCreditsText.rectTransform;
        creditsRect.anchorMin = new Vector2(0.5f, 0.5f);
        creditsRect.anchorMax = new Vector2(0.5f, 0.5f);
        creditsRect.pivot = new Vector2(0.5f, 0.5f);
        ApplyCreditsStyle();

        creditsRect.anchoredPosition = new Vector2(0f, -10000f);
        winCreditsText.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsRect);
        winCreditsText.ForceMeshUpdate();

        float lowerY = Mathf.Min(winCreditsStartY, winCreditsEndY);
        float upperY = Mathf.Max(winCreditsStartY, winCreditsEndY);
        float creditsHalfHeight = Mathf.Max(creditsRect.rect.height * 0.5f, 450f);
        float viewportHalfHeight = Screen.height * 0.5f;
        Canvas creditsCanvas = winCreditsText.canvas;
        if (creditsCanvas != null)
        {
            RectTransform canvasRect = creditsCanvas.transform as RectTransform;
            if (canvasRect != null)
            {
                viewportHalfHeight = Mathf.Max(viewportHalfHeight, canvasRect.rect.height * 0.5f);
            }
        }

        float offscreenBottomY = -(viewportHalfHeight + creditsHalfHeight + 40f);
        float offscreenTopY = viewportHalfHeight + creditsHalfHeight + 40f;
        lowerY = Mathf.Min(lowerY, offscreenBottomY);
        upperY = Mathf.Max(upperY, offscreenTopY);
        Vector2 startPos = new Vector2(0f, lowerY);
        Vector2 endPos = new Vector2(0f, upperY);

        float elapsed = 0f;
        bool restartPromptShown = false;
        creditsRect.anchoredPosition = startPos;
        creditsRect.localScale = Vector3.one * Mathf.Max(0.05f, starWarsStartScale);
        creditsRect.localRotation = Quaternion.identity;
        Color baseCreditsColor = winCreditsText.color;

        while (elapsed < winCreditsScrollDuration)
        {
            elapsed += Time.deltaTime;
            float tLinear = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, winCreditsScrollDuration));
            float t = EvaluateSequenceCurve(winCreditsProgressCurve, tLinear);
            creditsRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            if (useStarWarsCreditsStyle)
            {
                float crawlScale = Mathf.Lerp(starWarsStartScale, starWarsEndScale, t);
                creditsRect.localScale = Vector3.one * Mathf.Max(0.05f, crawlScale);
                creditsRect.localRotation = Quaternion.Euler(starWarsTiltEuler);
            }

            float alphaFactor = EvaluateSequenceCurve(winCreditsAlphaCurve, tLinear);
            Color c = baseCreditsColor;
            c.a = Mathf.Clamp01(baseCreditsColor.a * alphaFactor);
            winCreditsText.color = c;

            float promptTriggerTime = Mathf.Max(0f, winCreditsScrollDuration - restartPromptLeadTimeBeforeCreditsEnd);
            if (!restartPromptShown && elapsed >= promptTriggerTime)
            {
                ShowRestartPrompt();
                restartPromptShown = true;
            }

            yield return null;
        }

        creditsRect.anchoredPosition = endPos;
        winCreditsText.maxVisibleLines = int.MaxValue;
        if (useStarWarsCreditsStyle)
        {
            creditsRect.localScale = Vector3.one * Mathf.Max(0.05f, starWarsEndScale);
            creditsRect.localRotation = Quaternion.Euler(starWarsTiltEuler);
        }

        if (!restartPromptShown)
        {
            ShowRestartPrompt();
        }
        winCreditsCoroutine = null;
    }

    private System.Collections.IEnumerator RestartPromptBlinkRoutine()
    {
        if (restartPromptText == null) yield break;

        float minAlpha = Mathf.Clamp01(restartPromptMinAlpha);
        while (restartPromptText != null && restartPromptText.gameObject.activeInHierarchy)
        {
            float wave = (Mathf.Sin(Time.unscaledTime * restartPromptBlinkSpeed) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(minAlpha, 1f, wave);
            Color c = restartPromptText.color;
            c.a = alpha;
            restartPromptText.color = c;
            yield return null;
        }

        restartPromptBlinkCoroutine = null;
    }

    private void ApplyCreditsStyle()
    {
        if (winCreditsText == null) return;

        if (useStarWarsCreditsStyle)
        {
            winCreditsText.color = starWarsCreditsColor;
            winCreditsText.outlineColor = Color.black;
            winCreditsText.outlineWidth = 0.18f;
            winCreditsText.fontStyle |= FontStyles.Bold;
            winCreditsText.enableAutoSizing = false;
            winCreditsText.alignment = TextAlignmentOptions.Top;
        }
        else
        {
            winCreditsText.color = Color.white;
            winCreditsText.outlineColor = Color.black;
            winCreditsText.outlineWidth = 0.25f;
            winCreditsText.enableAutoSizing = false;
            winCreditsText.alignment = TextAlignmentOptions.Top;
        }
    }

    private string GetCreditsContentWithoutDuplicatedTitle()
    {
        string content = winCreditsContent;
        if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(winIntroTitleContent))
        {
            return content;
        }

        string normalized = content.Replace("\r\n", "\n");
        string trimmed = normalized.TrimStart('\n', '\r', ' ');
        string title = winIntroTitleContent.Trim();

        if (!trimmed.StartsWith(title, System.StringComparison.OrdinalIgnoreCase))
        {
            return content;
        }

        string remaining = trimmed.Substring(title.Length).TrimStart('\n', '\r', ' ');
        return string.IsNullOrWhiteSpace(remaining) ? content : remaining;
    }

    private string GetIntroTitleDisplayText()
    {
        if (string.IsNullOrWhiteSpace(winIntroTitleContent))
        {
            return winIntroTitleContent;
        }

        if (starWarsIntroTitleFont == null)
        {
            return winIntroTitleContent;
        }

        string uppercase = winIntroTitleContent.ToUpperInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder(uppercase.Length);
        for (int i = 0; i < uppercase.Length; i++)
        {
            char c = uppercase[i];
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private void ApplyBottomLeftLineLayout(RectTransform targetRect, int lineIndex)
    {
        if (targetRect == null) return;

        float yMin = hintBottomLeftMargin.y + (Mathf.Max(0, lineIndex) * (hintLineHeight + hintLineGap));
        targetRect.anchorMin = new Vector2(0f, 0f);
        targetRect.anchorMax = new Vector2(1f, 0f);
        targetRect.pivot = new Vector2(0.5f, 0f);
        targetRect.offsetMin = new Vector2(hintBottomLeftMargin.x, yMin);
        targetRect.offsetMax = new Vector2(-hintBottomLeftMargin.x, yMin + hintLineHeight);
    }
}