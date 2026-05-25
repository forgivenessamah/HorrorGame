using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public Animator anim;
    public Transform player;
    Vector3 dest;

    void Update()
    {
        if (pickupLetter.pagesCollected == 0)
        {
            if (ai.enabled)
                ai.enabled = false;
            return;
        }

        if (!ai.enabled)
            ai.enabled = true;

        dest = player.position;
        ai.destination = dest;

        if(pickupLetter.pagesCollected == 1)
        {
            ai.speed = 1.5f;
            anim.speed = 0.2f;
        }
        if (pickupLetter.pagesCollected == 2)
        {
            ai.speed = 1.7f;
            anim.speed = 0.4f;
        }
        if (pickupLetter.pagesCollected == 3)
        {
            ai.speed = 1.9f;
            anim.speed = 0.6f;
        }
        if (pickupLetter.pagesCollected == 4)
        {
            ai.speed = 2.5f;
            anim.speed = 0.8f;
        }
        if (pickupLetter.pagesCollected == 5)
        {
            ai.speed = 3f;
            anim.speed = 1f;
        }
        if (pickupLetter.pagesCollected == 6)
        {
            ai.speed = 3.5f;
            anim.speed = 1.2f;
        }
        if (pickupLetter.pagesCollected == 7)
        {
            ai.speed = 3.8f;
            anim.speed = 1.4f;
        }
        if (pickupLetter.pagesCollected == 8)
        {
            ai.speed = 4f;
            anim.speed = 1.6f;
        }
    }
}

