using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public GameObject Obj;
    public float activeTime;

    private bool isDisabling = false;

    void Update()
    {
        if (Obj != null && Obj.activeSelf && !isDisabling)
        {
            StartCoroutine(Disableobj());
        }
    }
    IEnumerator Disableobj()
    {
        isDisabling = true;
        yield return new WaitForSeconds(activeTime);
        if (Obj != null)
        {
            Obj.SetActive(false);
        }
        isDisabling = false;
    }
}