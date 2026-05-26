using System.Collections;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public GameObject Obj;
    public float activeTime;

    bool disableScheduled;

    void Update()
    {
        if (Obj == null || !Obj.activeSelf || disableScheduled)
            return;

        disableScheduled = true;
        StartCoroutine(Disableobj());
    }

    IEnumerator Disableobj()
    {
        yield return new WaitForSeconds(activeTime);
        if (Obj != null)
            Obj.SetActive(false);
        disableScheduled = false;
    }
}
