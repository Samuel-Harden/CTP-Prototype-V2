using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLot : MonoBehaviour
{
    [SerializeField] int divideCount = 1;

    private float lotWidth;
    private float lotLength;

    private bool divided;

    //private Vector3 boundsPos;

    private int noDivisions = 4;
    private int buildDepth = 2;
    private int counter;

    private int tileSize;

    private float lotWidthUpdated;
    private float lotLengthUpdated;

    private int division;

    private int panelSize = 1;

    private int lotIndex;

    // the min size a building can be
    private int minBuildingSize = 2;

    // The panels that make up the building
    private List<Panel> buildingPanels;

    private List<int> mutationDirections;

    private int offsetX;
    private int offsetZ;

    private int buildingWidth;
    private int buildingLength;
    private int buildingHeight;

    private int maxBuildingHeight;

    // The Length of each face
    // needed for mutations
    private int posXWidth;
    private int negXWidth;
    private int posZWidth;
    private int negZWidth;


    public void Initialise(Vector3 _pos, float _lotWidth, float _lotLength, int _maxDepth,
        GameObject _lotPrefab, int _tileSize, List<BuildingLot> _buildingLots,
        List<Vector3> _perlinPositions, int _division)
    {
        divided = false;
        lotWidth = _lotWidth;
        lotLength = _lotLength;
        tileSize = _tileSize;

        division = _division;

        if (_maxDepth > 0)
        {
            if (DivideCheck(_perlinPositions))
            {
                _division++;
                Divide(_perlinPositions, _maxDepth, _lotPrefab, _buildingLots, _tileSize, _division);
            }
        }
    }

    // This recalculates the lot size and factors in the roads
    public void RecalculateLotSize()
    {
        Vector3 updatedPos = transform.position;

        updatedPos.x += (float)tileSize / 2;
        updatedPos.z += (float)tileSize / 2;

        transform.position = updatedPos;

        lotLengthUpdated = lotLength - tileSize;
        lotWidthUpdated = lotWidth - tileSize;
    }


    public void CalculateBuildingData()
    {
        if (division > buildDepth)
        {
            // Get main building data first
            GenerateBuilding();
        }
    }


    public void SetSelectedBuilding()
    {
        GetComponentInParent<CityGen>().SetCurrentBuilding(lotIndex);
    }


    private void GenerateBuilding()
    {
        // Possibly add a rest here, then you could just call this to regen a building

        // ALL THIS BELOW COULD AND SHOULD GO INTO THE BUILDING CREATION KIT!!!

        // add offset for paths
        Vector3 pos = transform.position;
        pos.x += (float)panelSize / 2;
        pos.y += (float)panelSize / 2;
        pos.z += (float)panelSize / 2;

        lotWidthUpdated -= panelSize;
        lotLengthUpdated -= panelSize;

        transform.position = pos;

        buildingPanels = new List<Panel>();

        mutationDirections = new List<int>();

        // Scale min building size to == half the size of the lot
        int baseBuildingSizeX = (int)(lotWidthUpdated - 1) / 2;
        int baseBuildingSizeZ = (int)(lotLengthUpdated - 1) / 2;

        // Size
        buildingWidth = Random.Range(baseBuildingSizeX, ((int)lotWidthUpdated));
        buildingLength = Random.Range(baseBuildingSizeZ, ((int)lotLengthUpdated));

        //stops buildings being 1 wide or 1 length, and set it to min if too small
        if (buildingWidth < minBuildingSize)
            buildingWidth = minBuildingSize;

        if (buildingLength < minBuildingSize)
            buildingLength = minBuildingSize;

        // Set positional offset, so that not all buildings are central to their lot
        offsetX = Random.Range(0, ((int)lotWidthUpdated + 1) - buildingWidth);
        offsetZ = Random.Range(0, ((int)lotLengthUpdated + 1) - buildingLength);

        posXWidth = buildingLength;
        negXWidth = buildingLength;
        posZWidth = buildingWidth;
        negZWidth = buildingWidth;

        // Set Height
        buildingHeight = buildingLength + buildingWidth * 2;
        maxBuildingHeight = buildingHeight;

        BuildingCreationKit.GenerateBuilding(this);
    }


    private bool DivideCheck(List<Vector3> _perlinPositions)
    {
        counter = 0;

        foreach (Vector3 pos in _perlinPositions)
        {
            // is this position within bounds of node
            if (pos.x >= transform.position.x && pos.x < (transform.position.x + lotWidth) &&
                pos.z >= transform.position.z && pos.z < (transform.position.z + lotLength))
            {
                counter++;

                if (counter > divideCount)
                {
                    // if this lot has a pos in its bounds, we need to divide
                    divided = true;
                    return true;
                }
            }
        }

        // No positions found, we dont need to divide
        return false;
    }


    private void Divide(List<Vector3> _perlinPositions, int _maxDepth,
        GameObject _lotPrefab, List<BuildingLot> _buildingLots, int _tileSize, int _division)
    {
        _maxDepth -= 1;

        bool offsetWidth = false;
        bool offsetLength = false;
        bool sizeX = false;
        bool sizeZ = false;

        Vector3 newPos = transform.position;

        int count = 0;

        // Check if Lot needs an Offset
        offsetWidth = Offset(lotWidth, _tileSize);
        offsetLength = Offset(lotLength, _tileSize);

        // Order (Bottom left, Bottom right, Top left, Top right)
        for (int i = 0; i < noDivisions; i++)
        {
            float lotSizeX = lotWidth / 2;
            float lotSizeZ = lotLength / 2;

            if (offsetWidth)
            {
                if (sizeX)
                {
                    lotSizeX -= (float)_tileSize / 2;
                    sizeX = false;
                }

                else
                {
                    lotSizeX += (float)_tileSize / 2;
                    sizeX = true;
                }
            }

            if (offsetLength)
            {
                if (sizeZ)
                {
                    lotSizeZ -= (float)_tileSize / 2;
                }

                else
                {
                    lotSizeZ += (float)_tileSize / 2;
                }
            }

            GenerateLot(newPos, lotSizeX, lotSizeZ, _perlinPositions,
                _maxDepth, _lotPrefab, _buildingLots, _tileSize, _division);

            if (offsetWidth && sizeX)
                newPos.x += lotWidth / 2 + (float)_tileSize / 2;

            else
                newPos.x += lotWidth / 2;

            count++;

            if (count > 1)
            {
                newPos.x = transform.position.x;

                if (offsetLength)
                    newPos.z += lotLength / 2 + (float)_tileSize / 2;

                else
                    newPos.z += lotLength / 2;

                sizeZ = true;

                count = 0;
            }
        }
    }


    private void GenerateLot(Vector3 _newPos, float _lotSizeX, float _lotSizeZ,
        List<Vector3> _perlinPositions, int _maxDepth, GameObject _lotPrefab,
        List<BuildingLot> _buildingLots, int _tileSize, int _division)
    {
        var buildingLot = Instantiate(_lotPrefab, _newPos, Quaternion.identity);

        _buildingLots.Add(buildingLot.GetComponent<BuildingLot>());

        buildingLot.GetComponent<BuildingLot>().Initialise(_newPos, _lotSizeX,
            _lotSizeZ, _maxDepth, _lotPrefab, _tileSize, _buildingLots, _perlinPositions, _division);
    }


    private bool Offset(float _value, int _tileSize)
    {
        // check if offset is needed
        if ((_value / 2) % _tileSize != 0)
        {
            return true;
        }

        return false;
    }


    public int GetPosXWidth()
    {
        return posXWidth;
    }


    public void SetPosXWidth(int _width)
    {
        posXWidth = _width;
    }


    public int GetNegXWidth()
    {
        return negXWidth;
    }


    public void SetNegXWidth(int _width)
    {
        negXWidth = _width;
    }


    public int GetPosZWidth()
    {
        return posZWidth;
    }


    public void SetPosZWidth(int _width)
    {
        posZWidth = _width;
    }


    public int GetNegZWidth()
    {
        return negZWidth;
    }


    public void SetNegZWidth(int _width)
    {
        negZWidth = _width;
    }


    public List<Panel> GetBuildingPanels()
    {
        return buildingPanels;
    }


    public Panel GetBuildingPanel(int _index)
    {
        return buildingPanels[_index];
    }


    public List<int> GetMutationList()
    {
        return mutationDirections;
    }


    public int GetMaxBuildingHeight()
    {
        return maxBuildingHeight; // height of main building section
    }


    public int GetOffsetX()
    {
        return offsetX;
    }


    public void SetOffsetX(int _value)
    {
        offsetX = _value;
    }


    public int GetOffsetZ()
    {
        return offsetZ;
    }


    public void SetOffsetZ(int _value)
    {
        offsetZ = _value;
    }


    public float GetCurrentBuildingWidth()
    {
        return buildingWidth;
    }


    public float GetCurrentBuildingLength()
    {
        return buildingLength;
    }


    public float GetCurrentBuildingHeight()
    {
        return buildingHeight;
    }


    public void SetCurrentBuildingHeight(int _height)
    {
        buildingHeight = _height;
    }


    public float GetLotWidth()
    {
        return lotWidthUpdated;
    }


    public float GetLotLength()
    {
        return lotLengthUpdated;
    }


    public bool Divided()
    {
        return divided;
    }


    public float Width()
    {
        return lotWidth;
    }


    public float Length()
    {
        return lotLength;
    }


    public int LotIndex()
    {
        return lotIndex;
    }


    public void SetLotIndex(int _index)
    {
        lotIndex = _index;
    }


    public int Division()
    {
        return division;
    }

    // Deep copy values
    public void DeepCopyData(BuildingLot _lot)
    {
        transform.position = _lot.transform.position;

        lotWidth  = _lot.lotWidth;

        lotLength = _lot.lotLength;

        divided   = _lot.divided;

        counter   = _lot.counter;

        tileSize  = _lot.tileSize;

        lotWidthUpdated  = _lot.lotWidthUpdated;

        lotLengthUpdated = _lot.lotLengthUpdated;

        division = _lot.division;

        lotIndex = _lot.lotIndex;

        offsetX  = _lot.offsetX;

        offsetZ  = _lot.offsetZ;

        buildingWidth  = _lot.buildingWidth;

        buildingLength = _lot.buildingLength;

        buildingHeight = _lot.buildingHeight;

        maxBuildingHeight = _lot.maxBuildingHeight;
    }
}
