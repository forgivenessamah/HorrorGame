
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pickupLetter : MonoBehaviour
{
    public GameObject collectTextObj, intText, monster;
    public Transform playerTransform; // À ASSIGNER DANS L'INSPECTEUR (Glisse ton joueur ici)
    
    public AudioSource pickupSound, ambianceLayer1, ambianceLayer2, ambianceLayer3, ambianceLayer4, ambianceLayer5, ambianceLayer6, ambianceLayer7, ambianceLayer8;
    public bool interactable;
    public static int pagesCollected;
    public Text collectText;

    void Start()
    {
        pagesCollected = 0;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pagesCollected = pagesCollected + 1;
                
                // Correction de la syntaxe obsolète 'active' -> 'activeSelf'
                if (monster != null && !monster.activeSelf)
                {
                    if (playerTransform != null)
                    {
                        // Positionne le Slender à 25 unités derrière le joueur
                        Vector3 spawnPosition = playerTransform.position - (playerTransform.forward * 25f);
                        spawnPosition.y = playerTransform.position.y; // Aligné au sol
                        monster.transform.position = spawnPosition;
                    }

                    monster.SetActive(true);
                }

                collectText.text = pagesCollected + "/8 pages";
                collectTextObj.SetActive(true);
                pickupSound.Play();

                // Gestion des musiques d'ambiance
                if (pagesCollected == 1) ambianceLayer1.Play();
                if (pagesCollected == 2) ambianceLayer2.Play();
                if (pagesCollected == 3) ambianceLayer3.Play();
                if (pagesCollected == 4) ambianceLayer4.Play();
                if (pagesCollected == 5) ambianceLayer5.Play();
                if (pagesCollected == 6) ambianceLayer6.Play();
                if (pagesCollected == 7) ambianceLayer7.Play();
                if (pagesCollected == 8) ambianceLayer8.Play();

                intText.SetActive(false);
                interactable = false;
                this.gameObject.SetActive(false); // Désactive la page ramassée
            }
        }
    }
}