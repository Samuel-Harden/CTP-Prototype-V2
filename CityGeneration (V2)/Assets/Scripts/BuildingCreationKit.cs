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
        SetMutations(_lot);
        // Mutate!!!
        // remove duplicates!
    }


    private static void GenerateMain(BuildingLot _lot)
    {
        // PosX
        Vector3 pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() + _lot.GetNegZWidth()) +
            (float)panelSize / 2, ((float)panelSize / 2), _lot.GetPosZOffset() + panelSize);

        GeneratePosXPanels(_lot, (int)_lot.GetCurrentBuildingLength(), pos, _lot.GetBuildingPanels()); //gold

        // PosZ
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize, ((float)panelSize / 2),
            (_lot.GetPosZOffset() + _lot.GetPosXWidth()) + (float)panelSize / 2);

        GeneratePosZPanels(_lot, (int)_lot.GetCurrentBuildingWidth(), pos, _lot.GetBuildingPanels()); 

        // NegX
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + ((float)panelSize / 2),
            ((float)panelSize / 2), _lot.GetPosZOffset() + panelSize);

        GenerateNegXPanels(_lot, (int)_lot.GetCurrentBuildingLength(), pos, _lot.GetBuildingPanels());

        // NegZ
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize, ((float)panelSize / 2),
            _lot.GetPosZOffset() + ((float)panelSize / 2));

        GenerateNegZPanels(_lot, (int)_lot.GetCurrentBuildingWidth(), pos, _lot.GetBuildingPanels());

        // PosY
        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + panelSize,
            _lot.GetCurrentBuildingHeight(), _lot.GetPosZOffset() + panelSize);

        GeneratePosYPanels(_lot, (int)_lot.GetCurrentBuildingWidth(), (int)_lot.GetCurrentBuildingLength(),
            pos, _lot.GetBuildingPanels());
    }


    // Generates Panels for each face of building
    public static void GenerateNegXPanels(BuildingLot _lot, int _sectionWidth,
        Vector3 _startPos, List<Panel> _panels)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 270.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _sectionWidth; w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _panels.Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _sectionWidth;
            pos.y += panelSize;
        }
    }


    public static void GeneratePosXPanels(BuildingLot _lot, int _sectionWidth,
        Vector3 _startPos, List<Panel> _panels)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 90.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _sectionWidth; w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _panels.Add(panel);

                pos.z += panelSize;
            }

            pos.z -= _sectionWidth;
            pos.y += panelSize;
        }
    }


    public static void GeneratePosZPanels(BuildingLot _lot, int _sectionWidth,
        Vector3 _startPos, List<Panel> _panels)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 0.0f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _sectionWidth; w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _sectionWidth;
            pos.y += panelSize;
        }
    }


    public static void GenerateNegZPanels(BuildingLot _lot, int _sectionWidth,
        Vector3 _startPos, List<Panel> _panels)
    {
        Vector3 pos = _startPos;

        Vector3 rot = new Vector3(90.0f, 180f, 0.0f);

        for (int h = 0; h < _lot.GetCurrentBuildingHeight(); h++)
        {
            for (int w = 0; w < _sectionWidth; w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _sectionWidth;
            pos.y += panelSize;
        }
    }


    public static void GeneratePosYPanels(BuildingLot _lot, int _sectionWidth,
        int _sectionLength, Vector3 _startPos, List<Panel> _panels)
    {
        Vector3 pos = _startPos;

        Vector3 rot = Vector3.zero;

        for (int h = 0; h < _sectionLength; h++)
        {
            for (int w = 0; w < _sectionWidth; w++)
            {
                var panel = new Panel(h, w, false, pos, rot);

                _panels.Add(panel);

                pos.x += panelSize;
            }

            pos.x -= _sectionWidth;
            pos.z += panelSize;
        }
    }


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

        //maxMutations = (int)Random.Range(0, _lot.GetMutationList().Count + 1);

        if (_lot.GetMutationList().Count > 0) // JUST SET TO 1 FOR TESTING
            maxMutations = 1;

        Debug.Log(_lot.transform.position);

        while (/*counter < maxMutations*/_lot.GetMutationList().Count > 0)
        {
            // Randomly choose face
            int face = (int)Random.Range(0, _lot.GetMutationList().Count);

            int direction = _lot.GetMutationList()[face];

            DirectMutation(_lot, direction);

            //Remove possible mutation from list
            for (int i = 0; i < _lot.GetMutationList().Count; i++)
            {
                if (_lot.GetMutationList()[i] == direction)
                {
                    _lot.GetMutationList().RemoveAt(i);
                    continue;
                }
            }
        }

        // NEGATIVES NEED TO UPDATE THE OFFSET, OTHERWISE IF A POS MUTATION IS ADDED AFTER,
        // IT WILL BE OUT OF POSITION!!!!!!!!!
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
                GeneratePosXMutation(_lot);
                break;
            case 2:
                //GenerateNegZMutation(_lot);
                break;
            case 3:
                //GeneratePosZMutation(_lot);
                break;
        }
    }


    private static void MergeList(BuildingLot _lot, List<Panel> _addedPanels)
    {
        for (int i = _addedPanels.Count - 1; i >= 0; i--)
        {
            for (int j = _lot.GetBuildingPanels().Count - 1; j >= 0; j--)
            {
                // If panel is overlapping, discard, we dont need them, as they must be inside the mesh
                if (_addedPanels[i].Position() == _lot.GetBuildingPanel(j).Position())
                {
                    _lot.GetBuildingPanels().RemoveAt(j);
                    _addedPanels.RemoveAt(i);
                    continue;
                }
            }
        }

        // Add remaining panels
        foreach (Panel panel in _addedPanels)
        {
            _lot.GetBuildingPanels().Add(panel);
        }
    }


    private static void GenerateNegXMutation(BuildingLot _lot)
    {
        List<Panel> addedPanels = new List<Panel>();

        Debug.Log("NegX");

        //Debug.Log(_lot.transform.position);

        // set new height, width and length
        // Calculate size of mutation

        int newHeight = (int)_lot.GetCurrentBuildingHeight() / 2;

        newHeight = (int)Random.Range(newHeight, _lot.GetCurrentBuildingHeight() - 1);

        _lot.SetCurrentBuildingHeight(newHeight);

        //Debug.Log("Height: " + newHeight);

        int newWidth = (int)Random.Range(1, _lot.GetPosXOffset() + 1);

        //Debug.Log("Width: " + newWidth);

        int newLength = (int)Random.Range(1, _lot.GetNegXWidth() + 1);

        //Debug.Log("Length: " + newLength);

        // Generate panels!
        Vector3 pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() - newWidth) + (float)panelSize / 2,
            (float)panelSize / 2, _lot.GetPosZOffset() + panelSize); //gold

        GenerateNegXPanels(_lot, newLength, pos, addedPanels);

        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + (float)panelSize / 2,
            (float)panelSize / 2, _lot.GetPosZOffset() + panelSize); // gold

        GeneratePosXPanels(_lot, newLength, pos, addedPanels);

        pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() + panelSize) - newWidth,
            ((float)panelSize / 2), _lot.GetPosZOffset() + ((float)panelSize / 2));

        GenerateNegZPanels(_lot, newWidth, pos, addedPanels);

        pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() + panelSize) - newWidth,
            ((float)panelSize / 2), (_lot.GetPosZOffset() + newLength) + (float)panelSize / 2);

        GeneratePosZPanels(_lot, newWidth, pos, addedPanels);

        pos = _lot.transform.position + new Vector3((_lot.GetPosXOffset() + panelSize) - newWidth,
            newHeight, _lot.GetPosZOffset() + panelSize);

        //Debug.Log("xPos: " + ((_lot.GetPosXOffset() + panelSize) - newWidth));

        GeneratePosYPanels(_lot, newWidth, newLength, pos, addedPanels);

        // Update faceWidths
        _lot.SetNegZWidth(_lot.GetNegZWidth() + newWidth);

        if (_lot.GetNegXWidth() == newLength)
            _lot.SetPosZWidth(_lot.GetPosZWidth() + newWidth);

        _lot.SetPosXOffset(_lot.GetPosXOffset() - newWidth);

        MergeList(_lot, addedPanels);
    }


    // WORKING
    private static void GeneratePosXMutation(BuildingLot _lot)
    {
        List<Panel> addedPanels = new List<Panel>();

        Debug.Log("PosX");

        //Debug.Log(_lot.transform.position);

        // set new height, width and length
        // Calculate size of mutation

        int newHeight = (int)_lot.GetCurrentBuildingHeight() / 2;

        newHeight = (int)Random.Range(newHeight, _lot.GetCurrentBuildingHeight() - 1);

        _lot.SetCurrentBuildingHeight(newHeight);

        //Debug.Log("Height: " + newHeight);

        int newWidth = (int)Random.Range(1, (_lot.GetLotWidth() - (_lot.GetPosXOffset() + _lot.GetNegZWidth() + 1)));

        //Debug.Log("Width: " + newWidth);

        Debug.Log("maxWidth: " + (_lot.GetLotWidth() - (_lot.GetPosXOffset() + _lot.GetNegZWidth() + 1)));

        int newLength = (int)Random.Range(1, _lot.GetPosXWidth() + 1);

        //Debug.Log("Length: " + newLength);

        // Generate panels!
        Vector3 pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + _lot.GetNegZWidth()
            + (float)panelSize / 2, (float)panelSize / 2, _lot.GetPosZOffset() + panelSize);

        GenerateNegXPanels(_lot, newLength, pos, addedPanels);

        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + _lot.GetNegZWidth() + newWidth
            + (float)panelSize / 2, (float)panelSize / 2, _lot.GetPosZOffset() + panelSize);

        GeneratePosXPanels(_lot, newLength, pos, addedPanels);

        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + _lot.GetNegZWidth() + panelSize,
            ((float)panelSize / 2), _lot.GetPosZOffset() + ((float)panelSize / 2));

        GenerateNegZPanels(_lot, newWidth, pos, addedPanels);

        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + _lot.GetNegZWidth() + panelSize,
            ((float)panelSize / 2), (_lot.GetPosZOffset() + newLength) + (float)panelSize / 2);

        GeneratePosZPanels(_lot, newWidth, pos, addedPanels);

        pos = _lot.transform.position + new Vector3(_lot.GetPosXOffset() + _lot.GetNegZWidth()
            + panelSize, newHeight, _lot.GetPosZOffset() + panelSize);

        GeneratePosYPanels(_lot, newWidth, newLength, pos, addedPanels);

        // Update faceWidths
        _lot.SetNegZWidth(_lot.GetNegZWidth() + newWidth);

        // Only update this one, if updateLength = currentBuildinglength
        // That way if a mutation occurs on the PosZ face, it can etent to meet the new width for that face
        if (_lot.GetPosXWidth() == newLength)
            _lot.SetPosZWidth(_lot.GetPosZWidth() + newWidth);

        MergeList(_lot, addedPanels);
    }


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
            //Debug.Log("NegX: " + _lot.GetPosXOffset());
            return true;
        }

        return false;
    }

    // Note: LotWidth & Length factor in the path! So need to do a -panelSize for actual building area
    private static bool MutationPosX(BuildingLot _lot)
    {
        if (_lot.GetPosXOffset() + _lot.GetCurrentBuildingWidth() < (_lot.GetLotWidth() - panelSize))
        {
            //Debug.Log("PosX: " + ((_lot.GetLotWidth() - 1) - (_lot.GetPosXOffset() + _lot.GetCurrentBuildingWidth())));
            return true;
        }

        return false;
    }


    private static bool MutationNegZ(BuildingLot _lot)
    {
        if (_lot.GetPosZOffset() != 0)
        {
            //Debug.Log("NegZ: " + _lot.GetPosZOffset());
            return true;
        }

        return false;
    }


    private static bool MutationPosZ(BuildingLot _lot)
    {
        if (_lot.GetPosZOffset() + _lot.GetCurrentBuildingLength() < (_lot.GetLotLength() - panelSize))
        {
            //Debug.Log("PosZ: " + ((_lot.GetLotLength() - 1) - (_lot.GetPosZOffset() + _lot.GetCurrentBuildingLength())));
            return true;
        }

        return false;
    }
}
