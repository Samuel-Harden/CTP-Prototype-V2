using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingCreationKit
{
    private const int panelSize = 1;


    // Generates a list for each face of building
    public static List<Panel> GenerateNegXList(int _faceWidth, int _faceHeight, Vector3 _startPos)
    {
        List<Panel> panels = new List<Panel>();

        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 270.0f, 0.0f);

        for (int h = 0; h < _faceHeight; h++)
        {
            for (int w = 0; w < _faceWidth; w++)
            {
                var panel = new Panel(h, w, true, pos, rot);

                panels.Add(panel);

                pos.z -= panelSize;
            }

            pos.z += _faceWidth;
            pos.y += panelSize;
        }

        return panels;
    }


    public static List<Panel> GeneratePosXList(int _faceWidth, int _faceHeight, Vector3 _startPos)
    {
        List<Panel> panels = new List<Panel>();

        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 90.0f, 0.0f);

        for (int h = 0; h < _faceHeight; h++)
        {
            for (int w = 0; w < _faceWidth; w++)
            {
                var panel = new Panel(h, w, true, pos, rot);

                panels.Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _faceWidth;
            pos.y += panelSize;
        }

        return panels;
    }


    public static List<Panel> GeneratePosZList(int _faceWidth, int _faceHeight, Vector3 _startPos)
    {
        List<Panel> panels = new List<Panel>();

        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 0.0f, 0.0f);

        for (int h = 0; h < _faceHeight; h++)
        {
            for (int w = 0; w < _faceWidth; w++)
            {
                var panel = new Panel(h, w, true, pos, rot);

                panels.Add(panel);

                pos.x -= panelSize;
            }

            pos.x += _faceWidth;
            pos.y += panelSize;
        }

        return panels;
    }


    public static List<Panel> GenerateNegZList(int _faceWidth, int _faceHeight, Vector3 _startPos)
    {
        List<Panel> panels = new List<Panel>();

        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 180f, 0.0f);

        for (int h = 0; h < _faceHeight; h++)
        {
            for (int w = 0; w < _faceWidth; w++)
            {
                var panel = new Panel(h, w, true, pos, rot);

                panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _faceWidth;
            pos.y += panelSize;
        }

        return panels;
    }


    public static List<Panel> GeneratePosYList(int _faceWidth, int _faceHeight, Vector3 _startPos)
    {
        List<Panel> panels = new List<Panel>();

        Vector3 pos = _startPos;

        Vector3 rot = Vector3.zero;

        for (int h = 0; h < _faceHeight; h++)
        {
            for (int w = 0; w < _faceWidth; w++)
            {
                var panel = new Panel(h, w, true, pos, rot);

                panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _faceWidth;
            pos.z += panelSize;
        }

        return panels;
    }
}
