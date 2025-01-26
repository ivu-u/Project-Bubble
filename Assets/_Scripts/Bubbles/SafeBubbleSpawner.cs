using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SafeBubbleSpawner : MonoBehaviour
{
    #region Properties

    [SerializeField] [Range(0f, 1f)] float spawnChance = 0.33f;
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] GameObject safeBubblePrefab;
    Player _player;

    float timeElapsed = 0f;
    
    #endregion

    void Start()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= spawnInterval)
        {
            timeElapsed = 0f;
            if (Random.Range(0f, 1f) >= spawnChance)
            {
                SpawnBubble();
            }
        }
    }
    
    void SpawnBubble()
    {
        if (!safeBubblePrefab) return;
        if (!_player) return;

        Vector3 spawnCenter = _player.transform.position;
        
        // Screen space coords
        int width = Screen.width;
        int height = Screen.height;
        float xCoord = width * Random.Range(0f, 1f);
        float yCoord = height * Random.Range(0f, 0.1f);
        Vector3 randPointInScreenSpace = Camera.main.ScreenToWorldPoint(new Vector3(xCoord, yCoord, 0));
        randPointInScreenSpace.z = 0;

        // Spawn bubble
        GameObject spawnedBubble = Instantiate(safeBubblePrefab, randPointInScreenSpace, Quaternion.identity);
    }
}
