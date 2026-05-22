using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jumpscareTrig : MonoBehaviour
{
    public GameObject playerObj, jumpscareCam, ambianceLayers;
    public Animator monsterAnim;
    public string sceneName;
    public float jumpscareTime = 2.5f; // Valeur par défaut de sécurité si oublié dans l'inspecteur

    private bool hasTriggered = false; // Sécurité anti-double déclenchement

    void OnTriggerEnter(Collider other)
    {
        // Si le joueur touche le trigger et qu'on n'a pas déjà déclenché le jumpscare
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            playerObj.SetActive(false);
            jumpscareCam.SetActive(true);
            
            if (ambianceLayers != null) ambianceLayers.SetActive(false);
            
            if (monsterAnim != null) monsterAnim.SetTrigger("jumpscare");

            StartCoroutine(changeScene());
        }
    }

    IEnumerator changeScene()
    {
        // Attend le temps défini avant de reset
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(sceneName); 
    }
}