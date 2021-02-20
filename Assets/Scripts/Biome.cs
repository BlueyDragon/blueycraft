using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Blueycraft/Biome Attribute")]
public class Biome : ScriptableObject
{
    public string biomeName;        // Name of biome.
    public int solidGroundHeight;   // Below this height, always solid ground (at first).
    public int terrainHeight;       // Starting from solid ground height, the highest possible point of the terrain.
    public float terrainScale;      // For the Perlin noise generator.

    public Lode[] lodes;
}

[System.Serializable]
public class Lode
{
    public string lodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}