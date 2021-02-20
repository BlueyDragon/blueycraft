using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;

    // Properties
    public bool isActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }

    // Constructor
    public Chunk(ChunkCoord _coord, World _world)
    {
        // Initialize references to the world object and the chunk's coordinates within the world.
        coord = _coord;
        world = _world;

        // Set up the chunk's game object and its components.
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        // Set the mesh renderer's material using the public material from the world, and parent
        // the chunk to the world object to preserve hierarchy.
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        // Programmatically construct the chunk and its mesh.
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    // Functions
    void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
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
                    if(world.blocktypes[voxelMap[x,y,z]].isSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 ||
            y < 0 || y > VoxelData.ChunkHeight - 1 ||
            z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x,y,z))
            return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world.blocktypes[voxelMap[x, y, z]].isSolid;
    }

    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int face = 0; face < 6; face++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[face]))
            {
                /*for (int i = 0; i < 6; i++)
                {
                    int triangleIndex = VoxelData.voxelTriangles[f, i];
                    vertices.Add(VoxelData.voxelVertices[triangleIndex] + pos);
                    triangles.Add(vertexIndex);

                    uvs.Add(VoxelData.voxelUVs[i]);

                    vertexIndex++;
                }*/

                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[face, 0]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[face, 1]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[face, 2]]);
                vertices.Add(pos + VoxelData.voxelVertices[VoxelData.voxelTriangles[face, 3]]);

                AddTexture(world.blocktypes[blockID].GetTextureID(face));

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

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);
        float offset = VoxelData.NormalizedBlockTextureSize;

        x *= offset;
        y *= offset;

        // Skip this step if the texture atlas starts with ID 0 in the bottom left
        y = 1f - y - offset;

        uvs.Add(new Vector2(x, y));                     // 0,0
        uvs.Add(new Vector2(x, y + offset));            // 0,1
        uvs.Add(new Vector2(x + offset, y));            // 1,0
        uvs.Add(new Vector2(x + offset, y + offset));   // 1,1
    }

}

public class ChunkCoord
{
    public int x, z;

    public ChunkCoord(int _x, int _z)
    {
        x = _x; z = _z;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}
