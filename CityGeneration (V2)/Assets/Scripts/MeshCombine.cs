using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    public void CombineMeshes()
    {
        Quaternion old_rot = transform.rotation;
        Vector3 old_pos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Mesh final_mesh = new Mesh();

        CombineInstance[] combine = new CombineInstance[filters.Length];

        for(int i = 0; i < filters.Length; i++)
        {
            if (filters[i].transform == transform)
                continue;

            combine[i].subMeshIndex = 0;
            combine[i].mesh = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
        }

        final_mesh.CombineMeshes(combine);

        final_mesh.RecalculateBounds();
        final_mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = final_mesh;

        transform.rotation = old_rot;
        transform.position = old_pos;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

