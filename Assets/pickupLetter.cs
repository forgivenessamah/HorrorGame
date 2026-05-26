
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class pickupLetter : MonoBehaviour
{
    public GameObject collectTextObj, intText, monster;
    public Transform playerTransform;

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
        if (!other.CompareTag("MainCamera") || intText == null)
            return;

        intText.SetActive(true);
        interactable = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera") || intText == null)
            return;

        intText.SetActive(false);
        interactable = false;
    }

    void Update()
    {
        if (!interactable || !Input.GetKeyDown(KeyCode.E))
            return;

        pagesCollected = pagesCollected + 1;

        if (monster != null && pagesCollected == 1)
        {
            Vector3 spawnPosition = monster.transform.position;
            if (playerTransform != null)
            {
                spawnPosition = playerTransform.position - (playerTransform.forward * 25f);
                spawnPosition.y = playerTransform.position.y;
            }

            monster.transform.position = spawnPosition;

            NavMeshAgent agent = monster.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(spawnPosition);
            }

                monster.SetActive(true);
                    PlayerCatchHandler.NotifyMonsterSpawned();

                    if (HealthManager.Instance != null)
                        HealthManager.Instance.OnHuntStarted();
                }

        if (collectText != null)
            collectText.text = pagesCollected + "/8 pages";
        if (collectTextObj != null)
            collectTextObj.SetActive(true);
        if (pickupSound != null)
            pickupSound.Play();

        PlayAmbianceForPage(pagesCollected);

        if (intText != null)
            intText.SetActive(false);
        interactable = false;
        gameObject.SetActive(false);
    }

    void PlayAmbianceForPage(int page)
    {
        AudioSource layer = page switch
        {
            1 => ambianceLayer1,
            2 => ambianceLayer2,
            3 => ambianceLayer3,
            4 => ambianceLayer4,
            5 => ambianceLayer5,
            6 => ambianceLayer6,
            7 => ambianceLayer7,
            8 => ambianceLayer8,
            _ => null
        };

        if (layer != null)
            layer.Play();
    }
}
