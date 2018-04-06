﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel
{
    private Vector3 position;
    private Vector3 rotation;

    private int row;
    private int col;

    private bool panelStatus;


    public Panel(int _row, int _col, bool _status, Vector3 _pos, Vector3 _rot)
    {
        row = _row;
        col = _col;

        panelStatus = _status;

        position = _pos;
        rotation = _rot;
    }


    // Setters
    public void SetStatus(bool _status)
    {
        panelStatus = _status;
    }


    public void SetPosition(Vector3 _pos)
    {
        position = _pos;
    }


    // Getters
    public bool PanelStatus()
    {
        return panelStatus;
    }


    public Vector3 Position()
    {
        return position;
    }


    public Vector3 Rotation()
    {
        return rotation;
    }


    public int Row()
    {
        return row;
    }


    public int Col()
    {
        return col;
    }
}
