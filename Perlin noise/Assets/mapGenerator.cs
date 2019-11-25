using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class mapGenerator : MonoBehaviour {
    public enum GenerationType
    {
        RANDOM, PERLINNOISE 
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octave;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;
    public bool autoUpdate;
    public TerrainType[] regions;
    public TerrainType[] mineral;

    public void GenerateMap()
    {
        if (generationType == GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }else if(generationType == GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }
    }
    public void GenerateMapWithNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed , noiseScale, octave, persistance, lacunarity, offset);
        float[,] noiseMap2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + Random.Range(1,10), noiseScale, octave, persistance, lacunarity, offset);
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = noiseMap[x, y];
                /*if (rnd < 0.4f)
                {
                    FindTileFromRegion(-1);
                }
                else*/ if (rnd > 0.7f){

                    float rnd2 = noiseMap2[x, y];
                    customTileMap[y * mapWidth + x] = FindMineralFromRegion(rnd2);
                }
                else
                {
                    customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
                }
 
            }
        }
        setTileMap(customTileMap);
    }
    public void GenerateMapWithRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeight];

        for(int y = 0; y<mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        setTileMap(customTileMap);
    }
    public void setTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapWidth + x]);
            }
        }
    }
    public TileBase FindTileFromRegion(float rnd)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (rnd <= regions[i].height)
            {
                return regions[i].tile;
            }
        }
        return regions[0].tile;
    }
    public TileBase FindMineralFromRegion(float rnd)
    {
        for (int i = 0; i < mineral.Length; i++)
        {
            if (rnd <= mineral[i].height)
            {
                return mineral[i].tile;
            }
        }
        return mineral[0].tile;
    }
    private void OnValidate()
    {
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octave < 0)
        {
            octave = 0;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}