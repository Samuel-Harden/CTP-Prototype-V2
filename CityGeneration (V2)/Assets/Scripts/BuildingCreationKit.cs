using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingCreationKit
{
    private const int panelSize = 1;


    public static void GenerateBuilding(BuildingLot _lot)
    {
        GenerateMain(_lot);

        //Identify sides for mutations
        // Mutate!!!
        // remove duplicates!
    }


    private static void GenerateMain(BuildingLot _lot)
    {
        // PosX
        Vector3 pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() + _lot.GetPosZWidth()) +
            (float)panelSize / 2, ((float)panelSize / 2), _lot.GetPosZOffset() + panelSize);

        GeneratePosXPanels(_lot, pos); //gold

        // PosZ
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize, ((float)panelSize / 2),
            (_lot.GetPosZOffset() + _lot.GetPosXWidth()) + (float)panelSize / 2);

        GeneratePosZPanels(_lot, pos); 

        // NegX
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + ((float)panelSize / 2),
            ((float)panelSize / 2), _lot.GetPosZOffset() + panelSize);

        GenerateNegXPanels(_lot, pos);

        // NegZ
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize, ((float)panelSize / 2),
            _lot.GetPosZOffset() + ((float)panelSize / 2));

        GenerateNegZPanels(_lot, pos);

        // PosY
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize,
            _lot.GetCurrentBuildingHeight(), _lot.GetPosZOffset() + panelSize);

        GeneratePosYPanels(_lot, pos);
    }


    // Generates a list for each face of building
    public static void GenerateNegXPanels(BuildingLot _lot, Vector3 _startPos)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 270.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _lot.GetNegXWidth(); w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _lot.GetBuildingPanels().Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _lot.GetNegXWidth();
            pos.y += panelSize;
        }
    }


    public static void GeneratePosXPanels(BuildingLot _lot, Vector3 _startPos)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 90.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _lot.GetPosXWidth(); w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _lot.GetBuildingPanels().Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _lot.GetPosXWidth();
            pos.y += panelSize;
        }
    }


    public static void GeneratePosZPanels(BuildingLot _lot, Vector3 _startPos)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 0.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _lot.GetPosZWidth(); w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _lot.GetBuildingPanels().Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _lot.GetPosZWidth();
            pos.y += panelSize;
        }
    }


    public static void GenerateNegZPanels(BuildingLot _lot, Vector3 _startPos)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 180f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _lot.GetNegZWidth(); w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _lot.GetBuildingPanels().Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _lot.GetNegZWidth();
            pos.y += panelSize;
        }
    }


    public static void GeneratePosYPanels(BuildingLot _lot, Vector3 _startPos)
    {
        Vector3 pos = _startPos;

        Vector3 rot = Vector3.zero;

        for (int h = 0; h < _lot.GetCurrentBuildingLength(); h++)
        {
            for (int w = 0; w < _lot.GetCurrentBuildingWidth(); w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _lot.GetBuildingPanels().Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _lot.GetCurrentBuildingWidth();
            pos.z += panelSize;
        }
    }


    // Needs updating to compare both lists and remove duplicates
    public static void ClearDuplicatePanels(List<List<Panel>> _buildingPanels)
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


    /*public static void UpdateAllLists(BuildingLot _lot)
    {
        UpdatePosXPanelList(_lot);

        UpdateNegXPanelList(_lot);

        UpdatePosZPanelList(_lot);

        UpdateNegZPanelList(_lot);

        UpdatePosYPanelList(_lot);
    }*/


    /*// PosX face
    public static void UpdatePosXPanelList(BuildingLot _lot)
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
    public static void UpdateNegXPanelList(BuildingLot _lot)
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
    public static void UpdatePosZPanelList(BuildingLot _lot)
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
    public static void UpdateNegZPanelList(BuildingLot _lot)
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
    public static void UpdatePosYPanelList(BuildingLot _lot)
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
    }*/


    public static void SetMutations(BuildingLot _lot)
    {
        // Find out how many mutations this building can have
        // (max 1 for each size)
        
        if (MutationNegX(_lot))
            _lot.GetMutationList().Add(0); // NegX

        if (MutationPosX(_lot))
            _lot.GetMutationList().Add(1); // PosX

        if (MutationNegZ(_lot))
            _lot.GetMutationList().Add(2); // NegZ

        if (MutationPosZ(_lot))
            _lot.GetMutationList().Add(3); // PosZ

        Debug.Log("Building Max Mutations: " + _lot.GetMutationList().Count);

        //pass through how many possible mutations,
        // Randomly set no of mutations for building
        // randomly choose face to mutate, check if possible? if so apply change
        if (_lot.GetMutationList().Count > 0) // Cant mutation a building with no room!
            ApplyMutations(_lot);
    }


    private static void ApplyMutations(BuildingLot _lot)
    {
        int maxMutations = 0;

        //int maxMutations = (int)Random.Range(0, _lot.GetMutationList().Count + 1);

        if (_lot.GetMutationList().Count > 0) // JUST SET TO 1 FOR TESTING
            maxMutations = 1;

        //Debug.Log("Chosen No mutations : " + maxMutations);

        // Randomly choose face
        int ID = (int)Random.Range(0, _lot.GetMutationList().Count);

        int direction = _lot.GetMutationList()[ID];

        int counter = 0;

        while (counter < maxMutations)
        {
            DirectMutation(_lot, direction);
            counter++;
        }
    }


    private static void DirectMutation(BuildingLot _lot, int _direction)
    {
        // NegX = 0
        // PosX = 1
        // NegZ = 2
        // PosZ = 3

        switch (_direction)
        {
            case 0:
                GenerateNegXMutation(_lot);
                break;
            case 1:
                //GeneratePosXMutation(_lot);
                break;
            case 2:
                GenerateNegZMutation(_lot);
                break;
            case 3:
                GeneratePosZMutation(_lot);
                break;
        }
    }


    private static void GenerateNegXMutation(BuildingLot _lot)
    {
        //Debug.Log("Gen NegX Mutation");
    }


    /*private static void GeneratePosXMutation(BuildingLot _lot)
    {
         Debug.Log("Gen posX Mutation");

        // set a new height, set new width set new length
        // Calculate size of mutation

        int newHeight = (int)_lot.GetCurrentBuildingHeight() / 2;

        newHeight = (int)Random.Range(newHeight, _lot.GetCurrentBuildingHeight() - 1);

        Debug.Log("Height: " + newHeight);

        int newWidth = (int)Random.Range(1, (_lot.GetLotWidth() - (_lot.GetPosXOffset() + _lot.GetCurrentBuildingWidth())));

        Debug.Log("Width: " + newWidth);

        int newLength = (int)Random.Range(1, _lot.GetCurrentBuildingLength());

        Debug.Log("Length: " + newLength);

        UpdateAllLists(_lot);
    }*/


    private static void GenerateNegZMutation(BuildingLot _lot)
    {
        //Debug.Log("Gen NegZ Mutation");
    }


    private static void GeneratePosZMutation(BuildingLot _lot)
    {
        //Debug.Log("Gen PosZ Mutation");
    }


    private static bool MutationNegX(BuildingLot _lot)
    {
        if (_lot.GetPosXOffset() != 0)
        {
            return true;
        }

        return false;
    }


    private static bool MutationPosX(BuildingLot _lot)
    {
        if (_lot.GetPosXOffset() + _lot.GetCurrentBuildingWidth() < _lot.GetLotWidth())
        {
            return true;
        }

        return false;
    }


    private static bool MutationNegZ(BuildingLot _lot)
    {
        if (_lot.GetPosZOffset() != 0)
        {
            return true;
        }

        return false;
    }


    private static bool MutationPosZ(BuildingLot _lot)
    {
        if (_lot.GetPosZOffset() + _lot.GetCurrentBuildingLength() < _lot.GetLotLength())
        {
            return true;
        }

        return false;
    }
}
