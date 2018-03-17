using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGen : MonoBehaviour
{
    [SerializeField] Material pathMat;
    [SerializeField] Material parkMat;

    [SerializeField] List<Texture> roofTextures;
    [SerializeField] List<Texture> wallTextures;
    [SerializeField] List<Texture> slantTextures;

    [SerializeField] GameObject planeMeshPrefab;
    [SerializeField] GameObject cuboidMeshPrefab;

    [SerializeField] GameObject pavementContainer;
    [SerializeField] GameObject buildingContainer;

    private PlaneMesh planeMesh;
    private CuboidMesh cuboidMesh;

    private int minBuildDepth;
    private float roadHeight;

    public void GenerateLots(List<BuildingLot> _lots, int _minBuildDepth, float _roadHeight)
    {
        planeMesh = GetComponent<PlaneMesh>();
        cuboidMesh = GetComponent<CuboidMesh>();

        minBuildDepth = _minBuildDepth;

        roadHeight = 0.07f;

        GeneratePaths(_lots);

        GenerateBuildings(_lots);
    }


    private void GenerateBuildings(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            // Build Something
            if (lot.Division() > minBuildDepth)
            {
                float height = (int)Random.Range(3, 9);
                // build a base cube
                var building = cuboidMesh.GenerateCuboid(cuboidMeshPrefab, lot.WidthUpdated() - 1, lot.LengthUpdated() - 1, height);

                Vector3 pos = lot.transform.position;

                pos.x += lot.WidthUpdated() / 2;
                pos.z += lot.LengthUpdated() / 2;

                pos.y = height / 2 + 0.07f;

                building.transform.position = pos;

                SetTexture(building);

                building.transform.parent = buildingContainer.transform;
            }
        }
    }


    private void GeneratePaths(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            var path = planeMesh.GeneratePlane(planeMeshPrefab, lot.WidthUpdated(), lot.LengthUpdated());

            Vector3 pos = lot.transform.position;

            pos.x += lot.WidthUpdated() / 2;
            pos.z += lot.LengthUpdated() / 2;
            pos.y = roadHeight;

            path.transform.position = pos;

            if(lot.Division() > minBuildDepth)
            {
                path.GetComponent<Renderer>().material = pathMat;
            }

            else
                path.GetComponent<Renderer>().material = parkMat;

            path.transform.parent = pavementContainer.transform;
        }
    }


    private void SetTexture(GameObject _building)
    {
        int roof = Random.Range(0, roofTextures.Count);
        int wall = Random.Range(0, wallTextures.Count);

        _building.GetComponent<Renderer>().material.SetTexture("_RoofTex", roofTextures[roof]);
        _building.GetComponent<Renderer>().material.SetTexture("_WallTex", wallTextures[wall]);
    }
}
