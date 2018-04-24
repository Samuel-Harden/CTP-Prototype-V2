using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script has been modifed and adapted from
// http://wiki.unity3d.com/index.php/ProceduralPrimitives
// to fit the needs of this PCG Project


public class PlaneMesh : MonoBehaviour
{
    public GameObject GeneratePlane(GameObject _prefab, float _width, float _length)
    {
        var plane = Instantiate(_prefab, Vector3.zero, Quaternion.identity);

        MeshFilter filter_one = plane.gameObject.GetComponent<MeshFilter>();

        Mesh mesh_plane = filter_one.mesh;

        mesh_plane.Clear();

        float length = _length;
        float width = _width;
        int resX = 2;
        int resZ = 2;

        #region Vertices		
        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < normales.Length; n++)
            normales[n] = Vector3.up;
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < nbFaces; face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        mesh_plane.vertices = vertices;

        mesh_plane.normals = normales;

        mesh_plane.uv = uvs;

        mesh_plane.triangles = triangles;

        mesh_plane.RecalculateBounds();

        return plane;
    }
}
