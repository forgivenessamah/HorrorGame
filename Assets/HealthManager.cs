using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Slender: The Eight Pages style sanity. No drain until the first page is collected.
/// Drain rises with each page and when Slender is nearby or in view. Static is death-only (PlayerCatchHandler).
/// </summary>
public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [Header("Sanity")]
    public float maxSanity = 100f;
    public float currentSanity = 100f;

    [Header("Eight Pages - hunt drain (after page 1)")]
    [Tooltip("Passive sanity loss per second, multiplied by pages collected.")]
    public float drainPerPage = 0.35f;
    [Tooltip("Extra drain while Slender is within range (scales up when closer).")]
    public float maxProximityDrain = 2.2f;
    [Tooltip("Extra drain per second when you are looking at Slender.")]
    public float lookAtSlenderDrain = 1.5f;
    public float slenderAwareDistance = 18f;
    public float catchDistance = 1.25f;
    [Tooltip("How wide your view cone is for 'seeing' Slender.")]
    public float lookAngleDegrees = 40f;

    [Header("References")]
    public Transform playerTransform;
    public Transform slenderTransform;
    public Camera playerCamera;

    [Header("UI")]
    public Slider healthSlider;
    public Text healthText;
    public bool hideBarUntilHuntStarts = true;

    bool isDead;
    Image sanityFillImage;

    public float currentHealth
    {
        get => currentSanity;
        set => currentSanity = value;
    }

    public float maxHealth
    {
        get => maxSanity;
        set => maxSanity = value;
    }

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        GameObject managerObject = new GameObject("HealthManager");
        managerObject.AddComponent<HealthManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        currentSanity = maxSanity;

        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main;

        if (slenderTransform == null)
            FindSlender();

        if (healthSlider == null)
            CreateHealthUI();

        CacheFillImage();
        UpdateBarVisibility();
        UpdateUI();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (isDead)
            return;

        int pages = Mathf.Max(0, pickupLetter.pagesCollected);

        if (pages == 0)
        {
            currentSanity = maxSanity;
            UpdateBarVisibility();
            UpdateUI();
            return;
        }

        UpdateBarVisibility();

        if (slenderTransform == null)
            FindSlender();

        float drain = drainPerPage * pages;

        if (IsSlenderHunting())
        {
            float distance = Vector3.Distance(GetPlayerPosition(), slenderTransform.position);
            if (distance <= slenderAwareDistance)
            {
                float proximity = 1f - Mathf.Clamp01(distance / slenderAwareDistance);
                proximity = proximity * proximity;
                drain += maxProximityDrain * proximity * (0.6f + pages * 0.08f);

                if (distance <= catchDistance)
                    drain += maxProximityDrain * 2f;

                if (IsLookingAtSlender())
                    drain += lookAtSlenderDrain * (0.5f + proximity);
            }
        }

        currentSanity -= drain * Time.deltaTime;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);
        UpdateUI();

        if (currentSanity <= 0f)
            BeginDeath();
    }

    bool IsSlenderHunting()
    {
        return slenderTransform != null && slenderTransform.gameObject.activeInHierarchy;
    }

    Vector3 GetPlayerPosition()
    {
        if (playerTransform != null)
            return playerTransform.position;
        return transform.position;
    }

    bool IsLookingAtSlender()
    {
        if (slenderTransform == null)
            return false;

        Transform viewTransform = playerCamera != null ? playerCamera.transform : playerTransform;
        if (viewTransform == null)
            return false;

        Vector3 toSlender = slenderTransform.position - viewTransform.position;
        float dist = toSlender.magnitude;
        if (dist < 0.01f)
            return true;

        float angle = Vector3.Angle(viewTransform.forward, toSlender.normalized);
        return angle <= lookAngleDegrees;
    }

    void BeginDeath()
    {
        if (isDead)
            return;

        isDead = true;

        if (PlayerCatchHandler.Instance != null)
            PlayerCatchHandler.Instance.TriggerDeath();
        else
            SceneManager.LoadScene("scene_environment");
    }

    public void OnHuntStarted()
    {
        currentSanity = maxSanity;
        UpdateBarVisibility();
        UpdateUI();
    }

    void UpdateBarVisibility()
    {
        if (healthSlider == null)
            return;

        bool show = !hideBarUntilHuntStarts || pickupLetter.pagesCollected > 0;
        healthSlider.gameObject.SetActive(show);
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxSanity;
            healthSlider.value = currentSanity;
        }

        if (healthText != null)
        {
            int pages = pickupLetter.pagesCollected;
            healthText.text = pages == 0
                ? "Sanity: --"
                : $"Sanity: {Mathf.CeilToInt(currentSanity)} / {Mathf.CeilToInt(maxSanity)}";
        }

        UpdateSanityColor();
    }

    void UpdateSanityColor()
    {
        if (sanityFillImage == null)
            CacheFillImage();

        if (sanityFillImage == null)
            return;

        float t = 1f - (currentSanity / maxSanity);
        sanityFillImage.color = Color.Lerp(
            new Color(0.25f, 0.95f, 0.3f),
            new Color(0.95f, 0.15f, 0.1f),
            t);
    }

    void CacheFillImage()
    {
        if (healthSlider == null || healthSlider.fillRect == null)
            return;

        sanityFillImage = healthSlider.fillRect.GetComponent<Image>();
    }

    void FindSlender()
    {
        GameObject slenderObj = GameObject.Find("slender");
        if (slenderObj != null)
            slenderTransform = slenderObj.transform;
    }

    void CreateHealthUI()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        GameObject root = new GameObject("SanityBar", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.anchoredPosition = new Vector2(10f, -10f);
        rootRect.sizeDelta = new Vector2(280f, 28f);

        Image background = root.AddComponent<Image>();
        Sprite uiSprite = Resources.GetBuiltinResource<Sprite>("UISprite.psd");
        background.sprite = uiSprite;
        background.type = Image.Type.Sliced;
        background.color = new Color(0f, 0f, 0f, 0.65f);

        Slider slider = root.AddComponent<Slider>();
        slider.interactable = false;
        slider.navigation = new Navigation { mode = Navigation.Mode.None };
        slider.direction = Slider.Direction.LeftToRight;
        slider.targetGraphic = background;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5f, 5f);
        fillAreaRect.offsetMax = new Vector2(-5f, -5f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = fill.AddComponent<Image>();
        fillImage.sprite = uiSprite;
        fillImage.type = Image.Type.Sliced;
        fillImage.color = new Color(0.25f, 0.95f, 0.3f);

        slider.fillRect = fillRect;

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(root.transform, false);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0f, 0f);
        handleRect.anchorMax = new Vector2(0f, 1f);
        handleRect.sizeDelta = new Vector2(12f, 18f);
        handleRect.anchoredPosition = Vector2.zero;

        Image handleImage = handle.AddComponent<Image>();
        handleImage.sprite = uiSprite;
        handleImage.type = Image.Type.Sliced;
        handleImage.color = new Color(1f, 1f, 1f, 0.2f);

        slider.handleRect = handleRect;
        slider.value = currentSanity;

        GameObject labelObject = new GameObject("SanityText", typeof(RectTransform));
        labelObject.transform.SetParent(root.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 16;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = "Sanity: --";
        label.raycastTarget = false;

        healthSlider = slider;
        healthText = label;
        sanityFillImage = fillImage;
    }
}
