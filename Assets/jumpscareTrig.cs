using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpscareTrig : MonoBehaviour
{
    public GameObject playerObj, jumpscareCam, ambianceLayers;
    public Animator monsterAnim;
    public string sceneName;
    public float jumpscareTime;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerObj.SetActive(false);
            jumpscareCam.SetActive(true);
            ambianceLayers.SetActive(false);
            StartCoroutine(changeScene());
            monsterAnim.SetTrigger("jumpscare");
        }
    }

    IEnumerator changeScene()
    {
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(sceneName); // Correction ici !
    }
}