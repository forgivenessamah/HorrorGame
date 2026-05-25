using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float baseDrainRate = 0.12f;
    public float drainPerPage = 0.05f;
    public float baseRechargeRate = 0.05f;
    public float rechargeBonusPerPage = 0.03f;
    public float maxRechargeRate = 0.12f;
    public float alwaysDrainRate = 0.005f;
    public float drainAccelerationPerSecond = 0.002f;
    // Maximum added drain from the time-based escalation (prevents impossible spikes)
    public float maxTimeDrainBonus = 0.05f;
    public Transform playerTransform;
    public Transform slenderTransform;
    public float slenderDrainDistance = 15f;

    [Header("UI")]
    public Slider healthSlider;
    public Text healthText;

    bool isDead;

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
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (playerTransform == null && Camera.main != null)
            playerTransform = Camera.main.transform;

        if (slenderTransform == null)
            FindSlender();

        if (healthSlider == null)
            CreateHealthUI();

        UpdateUI();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    float pageTimeSinceCollected = 0f;

    void Update()
    {
        if (isDead)
            return;

        int pages = Mathf.Max(0, pickupLetter.pagesCollected);
        float drainRate = alwaysDrainRate;
        float rechargeRate = 0f;

        bool slenderNearby = false;
        float distanceToSlender = float.MaxValue;
        float drainMultiplier = 0f;

        if (playerTransform != null && slenderTransform != null)
        {
            Vector3 diff = playerTransform.position - slenderTransform.position;
            distanceToSlender = diff.magnitude;
            slenderNearby = distanceToSlender <= slenderDrainDistance;

            // Proximity-based drain scaling: closer = faster drain
            if (slenderNearby)
            {
                drainMultiplier = 1f - (distanceToSlender / slenderDrainDistance);
                drainMultiplier = Mathf.Clamp01(drainMultiplier);
            }

            // Force health to 0 when Slender is about to catch (< 1 unit away)
            if (distanceToSlender < 1f)
                currentHealth = 0f;
        }

        // No direct recharge from pages any more. Pages increase drain slightly.
        rechargeRate = 0f;
        if (pages > 0)
        {
            pageTimeSinceCollected += Time.deltaTime;
        }
        else
        {
            pageTimeSinceCollected = 0f;
        }

        if (slenderNearby)
        {
            // Health loses only a little at first, but much more when Slender is close.
            float proximityDrain = baseDrainRate * (0.12f + drainMultiplier * 0.4f);
            drainRate += proximityDrain;

            // Time-based escalation only matters when Slender is actually near.
            float pageFactor = Mathf.Pow(pages, 0.5f); // smooth growth with diminishing returns
            float timeDrainBonus = 0f;
            if (pages > 0)
            {
                timeDrainBonus = drainAccelerationPerSecond * pageTimeSinceCollected * pageFactor;
                timeDrainBonus = Mathf.Min(timeDrainBonus, maxTimeDrainBonus);
            }
            drainRate += timeDrainBonus;

            // Small additional drain per page when Slender is near.
            float perPageImmediateDrain = drainPerPage * 0.04f * pages;
            drainRate += perPageImmediateDrain;
        }
        else
        {
            // Very gentle drain when Slender is not nearby.
            float perPageImmediateDrain = drainPerPage * 0.02f * pages;
            drainRate += perPageImmediateDrain;
        }

        float netRate = drainRate - rechargeRate;
        netRate = Mathf.Max(0f, netRate);
        currentHealth -= netRate * Time.deltaTime;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        if (currentHealth <= 0f)
            BeginDeath();
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

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
            healthText.text = $"Health: {Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
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

        GameObject root = new GameObject("HealthBar", typeof(RectTransform));
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
        fillAreaRect.anchorMin = new Vector2(0f, 0f);
        fillAreaRect.anchorMax = new Vector2(1f, 1f);
        fillAreaRect.offsetMin = new Vector2(5f, 5f);
        fillAreaRect.offsetMax = new Vector2(-5f, -5f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = fill.AddComponent<Image>();
        fillImage.sprite = uiSprite;
        fillImage.type = Image.Type.Sliced;
        fillImage.color = new Color(0.2f, 0.95f, 0.2f, 0.95f);

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
        slider.value = currentHealth;

        GameObject labelObject = new GameObject("HealthText", typeof(RectTransform));
        labelObject.transform.SetParent(root.transform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 16;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = $"Health: {Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        label.raycastTarget = false;

        healthSlider = slider;
        healthText = label;
    }
}
