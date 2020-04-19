using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carton
{
    public GameObject carton;
    public float currentPassedTime;
    public bool spawned;
    public int id {get; private set;}

    public Carton(GameObject carton, int idx)
    {
        this.carton = carton;
        this.currentPassedTime = 0f;
        this.spawned = false;
        this.id = idx;
    }
}

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Objects")]
    public Player player;
    public GameObject cartonPrefab;
    public Transform[] spawningCartonSpots;

    [Header("Spawning timing")]
    public float spawnAfterSeconds;
    public float removeAfterSeconds;
    
    private Carton[] cartons;
    private bool startSpawner;

    void Start()
    {
        InitializeSpawner();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.startSpawner)
        {
            for (int i = 0; i < this.cartons.Length; i++)
            {
                // Update timers only if this carton is not being carried
                if (this.player.GetCartonCarryId() != i)
                {
                    // If this carton is spawned you have to remove it if time is up
                    if (this.cartons[i].spawned)
                    {
                        if (this.cartons[i].currentPassedTime > this.removeAfterSeconds)
                        {
                            RemoveCarton(i);
                        }
                    }
                    else
                    {
                        if (this.cartons[i].currentPassedTime > this.spawnAfterSeconds)
                        {
                            AddCarton(i);
                        }
                    }
                    this.cartons[i].currentPassedTime += Time.deltaTime;
                }
            }
        }
    }

    public void InitializeSpawner()
    {
        this.cartons = new Carton[this.spawningCartonSpots.Length];
        for (int i = 0; i < this.cartons.Length; i++)
        {
            this.cartons[i] = new Carton(Instantiate(this.cartonPrefab, this.spawningCartonSpots[i]), i);
            this.cartons[i].carton.SetActive(false);
        }
        this.startSpawner = true;
    }

    private void RemoveCarton(int i)
    {
        this.cartons[i].spawned = false;
        this.cartons[i].carton.SetActive(false);
        this.cartons[i].currentPassedTime = 0f;
    }

    private void AddCarton(int i)
    {
        this.cartons[i].spawned = true;
        this.cartons[i].carton.SetActive(true);
        this.cartons[i].currentPassedTime = 0f;

        this.cartons[i].carton.transform.position = this.spawningCartonSpots[i].position;
    }

    // get the carton with smallet reamining time before getting destroyed
    public Carton GetCarton()
    {
        float maxPassedTime = 0f;
        int indexCartonObj = -1;
        for (int i = 0; i < this.cartons.Length; i++)
        {
            Carton c = this.cartons[i];
            if (c.spawned)
            {
                if (maxPassedTime < c.currentPassedTime)
                {
                    maxPassedTime = c.currentPassedTime;
                    indexCartonObj = i;
                }
            }
        }

        if (indexCartonObj == -1) return null;
        return this.cartons[indexCartonObj];
    }

    // when player removes the carton it gets stored back here
    public void DisposeCarton(int cartonId)
    {
        Carton c = this.cartons[cartonId];
        c.spawned = false;
        c.currentPassedTime = 0;
        c.carton.transform.SetParent(this.spawningCartonSpots[cartonId]);
        c.carton.SetActive(false);
    }
}
