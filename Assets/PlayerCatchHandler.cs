using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerCatchHandler : MonoBehaviour
{
    public static PlayerCatchHandler Instance { get; private set; }

    public GameObject playerObj;
    public GameObject staticOverlay;
    public GameObject ambianceLayers;
    public VideoPlayer staticVideo;
    public VideoClip staticClip;
    public RenderTexture staticRenderTexture;
    public string sceneName = "scene_environment";
    public float staticDuration = 3f;
    public float catchGracePeriod = 4f;

    [Header("Jumpscare")]
    public jumpscareTrig jumpscare;

    static float monsterSpawnTime = -999f;
    bool hasTriggered;

    public static void NotifyMonsterSpawned()
    {
        monsterSpawnTime = Time.time;
    }

    public void TriggerDeath()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        if (HealthManager.Instance != null)
            HealthManager.Instance.currentHealth = 0f;

        StartCoroutine(DeathSequence());
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        monsterSpawnTime = -999f;

        if (staticOverlay != null)
            staticOverlay.SetActive(false);

        if (jumpscare == null)
            jumpscare = GetComponent<jumpscareTrig>();

        EnsureVideoPlayer();
    }

    void Update()
    {
        if (!hasTriggered && HealthManager.Instance != null && HealthManager.Instance.currentHealth <= 0f)
            TriggerDeath();
    }

    void EnsureVideoPlayer()
    {
        if (staticOverlay == null)
            return;

        if (staticVideo == null)
            staticVideo = staticOverlay.GetComponent<VideoPlayer>();

        if (staticVideo == null)
            staticVideo = staticOverlay.AddComponent<VideoPlayer>();

        if (staticVideo.clip == null && staticClip != null)
            staticVideo.clip = staticClip;

        if (staticRenderTexture != null)
        {
            staticVideo.renderMode = VideoRenderMode.RenderTexture;
            staticVideo.targetTexture = staticRenderTexture;
        }

        staticVideo.isLooping = true;
        staticVideo.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggered)
            return;

        if (pickupLetter.pagesCollected == 0)
            return;

        float gracePeriod = GetGracePeriodForPages(pickupLetter.pagesCollected);
        if (Time.time - monsterSpawnTime < gracePeriod)
            return;

        TriggerDeath();
    }

    float GetGracePeriodForPages(int pages)
    {
        return Mathf.Clamp(3.5f - (pages - 1) * 0.4f, 0.7f, 3.5f);
    }

    void FreezePlayer()
    {
        if (playerObj == null)
            return;

        SC_FPSController fps = playerObj.GetComponent<SC_FPSController>();
        if (fps != null)
        {
            fps.canMove = false;
            fps.enabled = false;
        }

        CharacterController controller = playerObj.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        flashlightMovement flashlight = playerObj.GetComponentInChildren<flashlightMovement>();
        if (flashlight != null)
            flashlight.enabled = false;

        FootstepSound footsteps = playerObj.GetComponentInChildren<FootstepSound>();
        if (footsteps != null)
            footsteps.enabled = false;

        foreach (Renderer renderer in playerObj.GetComponentsInChildren<Renderer>())
        {
            if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                renderer.enabled = false;
        }
    }

    IEnumerator DeathSequence()
    {
        FreezePlayer();

        if (jumpscare != null)
            yield return jumpscare.PlayJumpscare();
        else
        {
            if (ambianceLayers != null)
                ambianceLayers.SetActive(false);
        }

        EnsureVideoPlayer();

        if (staticOverlay != null)
            staticOverlay.SetActive(true);

        if (staticVideo != null)
        {
            staticVideo.Stop();
            staticVideo.Prepare();

            while (!staticVideo.isPrepared)
                yield return null;

            staticVideo.Play();
        }

        yield return new WaitForSeconds(staticDuration);

        pickupLetter.pagesCollected = 0;
        SceneManager.LoadScene(sceneName);
    }
}
