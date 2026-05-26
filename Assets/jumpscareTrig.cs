using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpscareTrig : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject jumpscareCam;
    public GameObject ambianceLayers;
    public Animator monsterAnim;
    public string sceneName;
    public float jumpscareTime = 2.5f;
    public bool reloadSceneAfterJumpscare;

    bool hasTriggered;

    void Awake()
    {
        hasTriggered = false;

        if (jumpscareCam != null)
            jumpscareCam.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggered)
            return;

        if (!TryBeginJumpscare())
            return;

        if (reloadSceneAfterJumpscare)
            StartCoroutine(JumpscareWaitThenReload());
        else
            StartCoroutine(JumpscareWait());
    }

    public bool TryBeginJumpscare()
    {
        if (hasTriggered)
            return false;

        hasTriggered = true;
        ApplyJumpscare();
        return true;
    }

    public IEnumerator PlayJumpscare()
    {
        if (!hasTriggered)
            TryBeginJumpscare();

        yield return new WaitForSeconds(jumpscareTime);
    }

    void ApplyJumpscare()
    {
        if (playerObj != null)
        {
            foreach (AudioListener listener in playerObj.GetComponentsInChildren<AudioListener>(true))
                listener.enabled = false;

            foreach (Camera cam in playerObj.GetComponentsInChildren<Camera>(true))
                cam.enabled = false;
        }

        if (jumpscareCam != null)
            jumpscareCam.SetActive(true);

        if (ambianceLayers != null)
            ambianceLayers.SetActive(false);

        if (monsterAnim != null)
            monsterAnim.SetTrigger("jumpscare");
    }

    IEnumerator JumpscareWait()
    {
        yield return new WaitForSeconds(jumpscareTime);
    }

    IEnumerator JumpscareWaitThenReload()
    {
        yield return new WaitForSeconds(jumpscareTime);
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}
