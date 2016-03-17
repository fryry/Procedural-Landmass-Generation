﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

    public const float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPos;
    int chunkSize;
    int chunksVisibleInViewDist;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleInLastUpdate = new List<TerrainChunk>();    

    void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleInLastUpdate.Count; i++)
        {
            terrainChunksVisibleInLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleInLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPos.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPos.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsViaible())
                    {
                        terrainChunksVisibleInLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObj;
        Vector2 pos;
        Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            pos = coord * size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);

            meshObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObj.transform.position = posV3;
            meshObj.transform.localScale = Vector3.one * size / 10f;
            meshObj.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
            bool visible = viewerDistFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObj.SetActive(visible);
        }

        public bool IsViaible()
        {
            return meshObj.activeSelf;
        }
    }
}