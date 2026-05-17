using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pickupLetter : MonoBehaviour
{
    public GameObject collectTextObj, intText;
    public AudioSource pickupSound, ambianceLayer1, ambianceLayer2, ambianceLayer3, ambianceLayer4, ambianceLayer5, ambianceLayer6, ambianceLayer7, ambianceLayer8;
    public bool interactable;
    public static int pagesCollected;
    public Text collectText;

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
                collectText.text = pagesCollected + "/8 pages";
                collectTextObj.SetActive(true);
                pickupSound.Play();
                if (pagesCollected == 1)
                {
                    ambianceLayer1.Play();
                }
                if (pagesCollected == 2)
                {
                    ambianceLayer2.Play();
                }
                if (pagesCollected == 3)
                {
                    ambianceLayer3.Play();
                }
                if (pagesCollected == 4)
                {
                    ambianceLayer4.Play();
                }
                if (pagesCollected == 5)
                {
                    ambianceLayer5.Play();
                }
                if (pagesCollected == 6)
                {
                    ambianceLayer6.Play();
                }
                if (pagesCollected == 7)
                {
                    ambianceLayer7.Play();
                }
                if (pagesCollected == 8)
                {
                    ambianceLayer8.Play();
                }
                intText.SetActive(false);
                this.gameObject.SetActive(false);
                interactable = false;
            }
        }
    }
}
