using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    bool[,,] voxelMap = new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    // Start is called before the first frame update
    void Start()
    {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }

    void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkWidth - 1 ||
            y < 0 || y > VoxelData.ChunkHeight - 1 ||
            z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;

        return voxelMap[x, y, z];
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int f = 0; f < 6; f++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[f]))
            {
                /*for (int i = 0; i < 6; i++)
                {
                    int triangleIndex = VoxelData.voxelTriangles[f, i];
                    vertices.Add(VoxelData.voxelVertices[triangleIndex] + pos);
                    triangles.Add(vertexIndex);

                    uvs.Add(VoxelData.voxelUVs[i]);

                    vertexIndex++;
                }*/
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[f, 0]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[f, 1]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[f, 2]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[f, 3]]);

                uvs.Add(VoxelData.voxelUVs[0]);
                uvs.Add(VoxelData.voxelUVs[1]);
                uvs.Add(VoxelData.voxelUVs[2]);
                uvs.Add(VoxelData.voxelUVs[3]);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    void CreateMesh()
    {
        // Create a mesh using the vertex, triangle, and uv lists created earlier in the for loop.
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

}
