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
                var panel = new Panel(h, w, false, pos, rot);

                panels.Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _faceWidth;
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
                var panel = new Panel(h, w, false, pos, rot);

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
                var panel = new Panel(h, w, false, pos, rot);

                panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _faceWidth;
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
                var panel = new Panel(h, w, false, pos, rot);

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
                var panel = new Panel(h, w, false, pos, rot);

                panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _faceWidth;
            pos.z += panelSize;
        }

        return panels;
    }


    public static void ClearUnusedPanels(List<List<Panel>> _buildingPanels)
    {
        // Loop through each list for each face
        for (int i = 0; i < _buildingPanels.Count; i++)
        {
            // Reverse iterate (as we may need to remove entry)
            for (int j = _buildingPanels[i].Count - 1; j >= 0; j--)
            {
                // Remove unused building panel
                if (!_buildingPanels[i][j].PanelStatus())
                {
                    _buildingPanels[i].RemoveAt(j);
                }
            }
        }
    }

    // PosX face
    public static void UpdatePosXPanelList(List<Panel> _panelList, int _lotLength, int _lotWidth,
        int _updateHeight, int _updateLength, int _updateWidth, int _posXOffset, int _posZOffset)
    {
        for (int h = 0; h < _updateHeight; h++)
        {
            for (int w = 0; w < _lotLength; w++)
            {
                // this is a panel we need to activate and move
                if (w >= _posZOffset && w < (_posZOffset + _updateLength))
                {
                    // Check opposite length and offset for reposition
                    _panelList[(h * _lotLength) + w].SetStatus(true);

                    Vector3 newPos = _panelList[h * _lotLength + w].Position();

                    float updateX = (newPos.x - _lotWidth) + (_posXOffset + _updateWidth);

                    newPos.x = updateX;

                    _panelList[h * _lotLength + w].SetPosition(newPos);
                }
            }
        }
    }


    // NegX face
    public static void UpdateNegXPanelList(List<Panel> _panelList, int _lotLength, int _lotWidth,
        int _updateHeight, int _updateLength, int _updateWidth, int _posXOffset, int _posZOffset)
    {
        for (int h = 0; h < _updateHeight; h++)
        {
            for (int w = 0; w < _lotLength; w++)
            {
                // this is a panel we need to activate and move
                if (w >= _posZOffset && w < (_posZOffset + _updateLength))
                {
                    // Check opposite length and offset for reposition
                    _panelList[(h * _lotLength) + w].SetStatus(true);

                    Vector3 newPos = _panelList[h * _lotLength + w].Position();

                    float updateX = newPos.x + _posXOffset;

                    newPos.x = updateX;

                    _panelList[h * _lotLength + w].SetPosition(newPos);
                }
            }
        }
    }


    // posZ face
    public static void UpdatePosZPanelList(List<Panel> _panelList, int _lotLength, int _lotWidth,
        int _updateHeight, int _updateLength, int _updateWidth, int _posXOffset, int _posZOffset)
    {
        for (int h = 0; h < _updateHeight; h++)
        {
            for (int w = 0; w < _lotWidth; w++)
            {
                // this is a panel we need to activate and move
                if (w >= _posXOffset && w < (_posXOffset + _updateWidth))
                {
                    // Check opposite length and offset for reposition
                    _panelList[(h * _lotWidth) + w].SetStatus(true);

                    Vector3 newPos = _panelList[h * _lotWidth + w].Position();

                    float updateZ = (newPos.z - _lotLength) + (_posZOffset + _updateLength);

                    newPos.z = updateZ;

                    _panelList[h * _lotWidth + w].SetPosition(newPos);
                }
            }
        }
    }


    // posZ face
    public static void UpdateNegZPanelList(List<Panel> _panelList, int _lotLength, int _lotWidth,
        int _updateHeight, int _updateLength, int _updateWidth, int _posXOffset, int _posZOffset)
    {
        for (int h = 0; h < _updateHeight; h++)
        {
            for (int w = 0; w < _lotWidth; w++)
            {
                // this is a panel we need to activate and move
                if (w >= _posXOffset && w < (_posXOffset + _updateWidth))
                {
                    // Check opposite length and offset for reposition
                    _panelList[(h * _lotWidth) + w].SetStatus(true);

                    Vector3 newPos = _panelList[h * _lotWidth + w].Position();

                    float updateZ = newPos.z + _posZOffset;

                    newPos.z = updateZ;

                    _panelList[h * _lotWidth + w].SetPosition(newPos);
                }
            }
        }
    }


    // posY face (Roof)
    public static void UpdatePosYPanelList(List<Panel> _panelList, int _lotLength, int _lotWidth,
        int _updateHeight, int _updateLength, int _updateWidth, int _posXOffset, int _posZOffset)
    {
        for (int h = 0; h < _lotLength; h++)
        {
            for (int w = 0; w < _lotWidth; w++)
            {
                // this is a panel we need to activate and move
                if (w >= _posXOffset && w < (_posXOffset + _updateWidth) &&
                    h >= _posZOffset && h < (_posZOffset + _updateLength))
                {
                    // Check opposite length and offset for reposition
                    _panelList[(h * _lotWidth) + w].SetStatus(true);

                    Vector3 newPos = _panelList[h * _lotWidth + w].Position();

                    newPos.y = _updateHeight;

                    _panelList[h * _lotWidth + w].SetPosition(newPos);
                }
            }
        }
    }
}
