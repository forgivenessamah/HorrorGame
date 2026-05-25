using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slenderAI : MonoBehaviour
{
    public Transform dest1, dest2, dest3, dest4, dest5, dest6, dest7, dest8, dest9, dest10, dest11, dest12, dest13, dest14, dest15, player;
    public float teleportRate;
    int randNum;
    bool teleporting = true;

    void Start()
    {
        StartCoroutine(teleportRoutine());
    }
    void Update()
    {
        this.transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
    }
    IEnumerator teleportRoutine()
    {
        while(teleporting == true)
        {
            yield return new WaitForSeconds(teleportRate);
            randNum = Random.Range(0, 15);
            if(randNum == 0)
            {
                this.transform.position = dest1.position;
            }
            if (randNum == 1)
            {
                this.transform.position = dest2.position;
            }
            if (randNum == 2)
            {
                this.transform.position = dest3.position;
            }
            if (randNum == 3)
            {
                this.transform.position = dest4.position;
            }
            if (randNum == 4)
            {
                this.transform.position = dest5.position;
            }
            if (randNum == 5)
            {
                this.transform.position = dest6.position;
            }
            if (randNum == 6)
            {
                this.transform.position = dest7.position;
            }
            if (randNum == 7)
            {
                this.transform.position = dest8.position;
            }
            if (randNum == 8)
            {
                this.transform.position = dest9.position;
            }
            if (randNum == 9)
            {
                this.transform.position = dest10.position;
            }
            if (randNum == 10)
            {
                this.transform.position = dest11.position;
            }
            if (randNum == 11)
            {
                this.transform.position = dest12.position;
            }
            if (randNum == 12)
            {
                this.transform.position = dest13.position;
            }
            if (randNum == 13)
            {
                this.transform.position = dest14.position;
            }
            if (randNum == 14)
            {
                this.transform.position = dest15.position;
            }
        }
    }
} 