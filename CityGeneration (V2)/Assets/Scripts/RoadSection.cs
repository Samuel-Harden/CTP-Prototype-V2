using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSection : MonoBehaviour
{
    [Space]
    [SerializeField] List<Vector3> aiWaypoints;

    [Space]
    [SerializeField] float wayPointOffset = 0.3f;

    [Space]
    [SerializeField] bool gizmosEnabled;

    private List<RoadSection> neighbours;

    private int index;
    private int row;
    private int col;


    public void SetData(int _col, int _row)
    {
        row = _row;
        col = _col;
    }


    public void SetWaypoints()
    {
        if(index == 9)
        {
            aiWaypoints.Add(new Vector3(transform.position.x - wayPointOffset, 0.25f, transform.position.z - 0.5f));
            aiWaypoints.Add(new Vector3(transform.position.x + wayPointOffset, 0.25f, transform.position.z - 0.5f));
            aiWaypoints.Add(new Vector3(transform.position.x - wayPointOffset, 0.25f, transform.position.z + 0.5f));
            aiWaypoints.Add(new Vector3(transform.position.x + wayPointOffset, 0.25f, transform.position.z + 0.5f));
        }

        else
        {
            aiWaypoints.Add(new Vector3(transform.position.x - 0.5f, 0.25f, transform.position.z - wayPointOffset));
            aiWaypoints.Add(new Vector3(transform.position.x + 0.5f, 0.25f, transform.position.z - wayPointOffset));
            aiWaypoints.Add(new Vector3(transform.position.x - 0.5f, 0.25f, transform.position.z + wayPointOffset));
            aiWaypoints.Add(new Vector3(transform.position.x + 0.5f, 0.25f, transform.position.z + wayPointOffset));
        }
    }


    // Pass in vehicle direction,
    // return next waypoint relavent to facing
    public List<Vector3> GetWaypoints(string _direction)
    {
        List<Vector3> positions = new List<Vector3>();

        // 6 is x axis
        // 9 is z axis
        if(index == 6)
        {
            if (_direction == "posX")
            {
                positions.Add(aiWaypoints[2]);
                positions.Add(aiWaypoints[3]);
                return positions;
            }

            if (_direction == "negX")
            {
                positions.Add(aiWaypoints[1]);
                positions.Add(aiWaypoints[0]);
                return positions;
            }
        }

        if (_direction == "posZ")
        {
            positions.Add(aiWaypoints[0]);
            positions.Add(aiWaypoints[2]);
            return positions;
        }

        //if (_direction == "negZ")
        positions.Add(aiWaypoints[3]);
        positions.Add(aiWaypoints[1]);

        return positions;
    }


    public void SetNeighbours(List<RoadSection> _neighbours)
    {
        neighbours = _neighbours;
    }


    public List<RoadSection> GetNeighbours()
    {
        return neighbours;
    }


    public void SetIndex(int _index)
    {
        index = _index;
    }


    public int Index()
    {
        return index;
    }


    public int Row()
    {
        return row;
    }


    public int Col()
    {
        return col;
    }


    private void OnDrawGizmos()
    {
        if (gizmosEnabled)
        {
            Gizmos.color = Color.green;

            if (aiWaypoints.Count != 0)
            {
                foreach (Vector3 pos in aiWaypoints)
                {
                    Gizmos.DrawWireSphere(pos, 0.1f);
                }
            }
        }
    }
}
