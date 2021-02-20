using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeInChunks = 10;

    public static int WorldSizeInVoxels
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks = 5;

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] voxelVertices = new Vector3[8]
    {
        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(1.0f,1.0f,0.0f),
        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(1.0f,1.0f,1.0f),
        new Vector3(0.0f,1.0f,1.0f)
    };

    public static readonly int[,] voxelTriangles = new int[6, 4]
    {
        // Full lookup table for vertex indices. They follow a specific pattern in order to
        // draw the two triangles of a face. The fourth and fifth are duplicates and can be
        // safely removed, as long as the pattern is preserved.
        //{0,3,1,1,3,2},  // Back Face
        //{5,6,4,4,6,7},  // Front Face
        //{3,7,2,2,7,6},  // Top Face
        //{1,5,0,0,5,4},  // Bottom Face
        //{4,7,0,0,7,3},  // Left Face
        //{1,2,5,5,2,6}   // Right Face

        {0,3,1,2},  // Back Face
        {5,6,4,7},  // Front Face
        {3,7,2,6},  // Top Face
        {1,5,0,4},  // Bottom Face
        {4,7,0,3},  // Left Face
        {1,2,5,6}   // Right Face
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        // This order is important; it corresponds to the order that the faces were constructed
        // in the previous array (back for the back face, forward for the front face, etc.)
        // Vector3 shorthand properties are used here, but they correspond to offset locations
        // either 1 or -1 in each of the three axes.
        Vector3.back,       // -1 Z
        Vector3.forward,    // 1 Z
        Vector3.up,         // 1 Y
        Vector3.down,       // -1 Y
        Vector3.left,       // -1 X
        Vector3.right       // 1 X
    };

    public static readonly Vector2[] voxelUVs = new Vector2[4]
    {
        // Full lookup table for UV indices. They follow the same pattern as the triangle
        // pattern above. As before, the fourth and fifth are duplicates and can be removed
        // for performance.

        // Textures are normalized - that is, 0 is the beginning and 1 is the end regardless
        // of what size the texture is. (0,0) is bottom left, (1,1) is top right. These vector
        // addresses correspond to the given vertex indices of a face's two triangles.
        //new Vector2(0.0f, 0.0f),    // Bottom Left of texture (beginning of first triangle)
        //new Vector2(0.0f, 1.0f),    // Top Left
        //new Vector2(1.0f, 0.0f),    // Bottom Right
        //new Vector2(1.0f, 0.0f),    // Bottom Right (beginning of second triangle)
        //new Vector2(0.0f, 1.0f),    // Top Left
        //new Vector2(1.0f, 1.0f)     // Top Right

        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f)
    };

}
