
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrapSpawner : MonoBehaviour
{
    public static TrapSpawner Instance;
    
    public float padding = 2;
    public List<GameObject> trapPrefabs;
    public List<GameObject> traps;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        traps = new List<GameObject>();
    }

    public void SpawnTrap()
    {
        if (trapPrefabs.Count == 0) return;
        traps.RemoveAll(x => x == null);
        var rnd = Random.Range(0, trapPrefabs.Count);
        if (traps.Count > 0)
        {
            var obj = Instantiate(trapPrefabs[rnd], transform);
            var lastPos = traps[traps.Count - 1].transform.position;
            obj.transform.position = lastPos - new Vector3(0, padding, 0);
            traps.Add(obj);
        }
        else
        {
            var obj = Instantiate(trapPrefabs[rnd], transform);
            traps.Add(obj);
        }     
    }
}
