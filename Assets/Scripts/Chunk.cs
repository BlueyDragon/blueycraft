using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int f = 0; f < 6; f++)
        {
            for (int i = 0; i < 6; i++)
            {
                int triangleIndex = VoxelData.voxelTriangles[f, i];
                vertices.Add(VoxelData.voxelVertices[triangleIndex]);
                triangles.Add(vertexIndex);

                uvs.Add(VoxelData.voxelUVs[i]);

                vertexIndex++;
            }
        }

        // Create a mesh using the vertex, triangle, and uv lists created earlier in the for loop.
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }


}
