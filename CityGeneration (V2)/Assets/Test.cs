using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public bool outline_enabled
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

    [SerializeField] List<Outline> outlines;

    private void Awake()
    {
        outline_enabled = false;
    }

    private void OnMouseOver()
    {
        Debug.Log("Moused over Cube");
        if (!EventSystem.current.IsPointerOverGameObject())
            outline_enabled = true;
    }


    private void OnMouseExit()
    {
        Debug.Log("Mouse left Cube");
        if (!EventSystem.current.IsPointerOverGameObject())
            outline_enabled = false;
    }
}
