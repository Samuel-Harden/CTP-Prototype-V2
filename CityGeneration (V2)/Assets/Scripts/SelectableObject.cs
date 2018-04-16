using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
            GetComponentInParent<BuildingLot>().SetSelectedBuilding();
    }
}
