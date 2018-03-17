using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] Material redStd;
    [SerializeField] Material grnStd;

    [SerializeField] Material redEm;
    [SerializeField] Material grnEm;

    private BoxCollider coll;
    private Vector3 resetPos;
    private MeshRenderer rend;
    private bool trafficWaiting;

    public void Initialise()
    {
        coll = GetComponentInChildren<BoxCollider>();
        resetPos = coll.transform.position;

        rend = GetComponent<MeshRenderer>();
    }


    public void TrafficWaiting(bool _state)
    {
        trafficWaiting = _state;
    }


    public bool IsTrafficWaiting()
    {
        return trafficWaiting;
    }


    public void SetLight(bool _active)
    {
        // green Light
        if (_active)
        {
            //guna enable collider and set light
            coll.transform.position = new Vector3(coll.transform.position.x, 2.0f, coll.transform.position.z);

            Material[] matArray = rend.materials;

            matArray[1] = grnEm;
            matArray[2] = redStd;

            rend.materials = matArray;

            return;
        }

        // red Light
        if (!_active)
        {
            // guna disable collider and set light
            coll.transform.position = resetPos;

            Material[] matArray = rend.materials;

            matArray[1] = grnStd;
            matArray[2] = redEm;

            rend.materials = matArray;

            return;
        }
    }
}
