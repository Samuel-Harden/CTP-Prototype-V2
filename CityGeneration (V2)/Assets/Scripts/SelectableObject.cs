using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using cakeslice;


public class SelectableObject : MonoBehaviour
{
    [SerializeField] List<Outline> outlines;


    private void Awake()
    {
        outlineEnabled = false;
    }


    public bool outlineEnabled
    {
        get
        {
            return outlines.TrueForAll(elem => elem.enabled);
        }

        set
        {
            outlines.ForEach(elem => elem.enabled = value);
        }
    }


    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GetComponentInParent<BuildingLot>().SetSelectedBuilding();
        }
    }


    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !outlineEnabled)
            outlineEnabled = true;
    }


    private void OnMouseExit()
    {
        if (!GetComponentInParent<BuildingLot>().CheckSelectedBuilding())
            outlineEnabled = false;
    }
}
