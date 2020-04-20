using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carton
{
    public GameObject carton;
    public float currentPassedTime;
    public float removeSpawnAfterSeconds;
    public bool spawned;
    public int id {get; private set;}

    public Carton(GameObject carton, int idx, float removeSpawnAfterSeconds)
    {
        this.carton = carton;
        this.currentPassedTime = 0f;
        this.removeSpawnAfterSeconds = removeSpawnAfterSeconds;
        this.spawned = false;
        this.id = idx;
    }
}

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Objects")]
    public Player player;
    public GameObject cartonPrefab;
    public GameObject beanPrefab;
    public Transform[] spawningCartonSpots;
    public BarLoader[] cartonLoaders;

    public Transform spawningBeanSpot;
    private Queue<Bean> beans;
    private int maxAmountOfBeans;
    private Queue<Bean> currentlyAvailableBeans;

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
                        if (this.cartons[i].currentPassedTime > this.cartons[i].removeSpawnAfterSeconds)
                        {
                            RemoveCarton(i);
                        }
                    }
                    else
                    {
                        if (this.cartons[i].currentPassedTime > this.cartons[i].removeSpawnAfterSeconds)
                        {
                            AddCarton(i);
                        }
                    }
                    this.cartonLoaders[i].Step(Time.deltaTime);
                    this.cartons[i].currentPassedTime += Time.deltaTime;
                }
            }
        }
    }

    public void InitializeSpawner()
    {
        // Cartons
        this.cartons = new Carton[this.spawningCartonSpots.Length];
        for (int i = 0; i < this.cartons.Length; i++)
        {
            this.cartons[i] = new Carton(Instantiate(this.cartonPrefab, this.spawningCartonSpots[i]), 
                                         i, 
                                         this.spawnAfterSeconds + Random.Range(-2f, 2f));
            this.cartons[i].carton.SetActive(false);
            this.cartonLoaders[i].Reset(0f, this.cartons[i].removeSpawnAfterSeconds, false, false);
        }

        // Beans
        this.maxAmountOfBeans = 10;
        this.beans = new Queue<Bean>();
        this.currentlyAvailableBeans = new Queue<Bean>();
        for (int i = 0; i < this.maxAmountOfBeans; i++)
        {
            GameObject beanGo = Instantiate(this.beanPrefab);
            beanGo.SetActive(false);
            this.beans.Enqueue(beanGo.GetComponent<Bean>());
        }

        this.startSpawner = true;
    }

    // ===============
    // CARTON
    // ===============
    private void RemoveCarton(int i)
    {
        this.cartons[i].spawned = false;
        this.cartons[i].carton.SetActive(false);
        this.cartons[i].currentPassedTime = 0f;
        this.cartons[i].removeSpawnAfterSeconds = this.spawnAfterSeconds + Random.Range(-2f, 2f);

        this.cartonLoaders[i].Reset(0f, this.cartons[i].removeSpawnAfterSeconds, false, false);
    }

    private void AddCarton(int i)
    {
        this.cartons[i].spawned = true;
        this.cartons[i].carton.SetActive(true);
        this.cartons[i].currentPassedTime = 0f;
        this.cartons[i].removeSpawnAfterSeconds = this.removeAfterSeconds + Random.Range(-2f, 2f);

        this.cartons[i].carton.transform.position = this.spawningCartonSpots[i].position;
        this.cartonLoaders[i].Reset(0f, this.cartons[i].removeSpawnAfterSeconds, true, true);
    }

    // get the carton with smallet reamining time before getting destroyed
    public Carton GetCarton()
    {
        if (this.currentlyAvailableBeans.Count >= this.maxAmountOfBeans) return null;

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
        else 
        {
            this.cartonLoaders[indexCartonObj].Reset(0f, 0f, false, false);
            return this.cartons[indexCartonObj];
        }
    }

    // when player removes the carton it gets stored back here
    public void DisposeCarton(int cartonId)
    {
        Carton c = this.cartons[cartonId];
        c.spawned = false;
        c.currentPassedTime = 0;
        c.carton.transform.SetParent(this.spawningCartonSpots[cartonId]);
        c.carton.SetActive(false);

        this.cartons[cartonId].removeSpawnAfterSeconds = this.spawnAfterSeconds + Random.Range(-2f, 2f);
        this.cartonLoaders[cartonId].Reset(0f, c.removeSpawnAfterSeconds, false, false);
    }

    // ===============
    // BEANS
    // ===============
    public void SpawnBean()
    {
        Bean bean = this.beans.Dequeue();
        if (bean != null)
        {
            bean.ShootBean(spawningBeanSpot);
            this.currentlyAvailableBeans.Enqueue(bean);
        }
    }


    // When player gets the beans from the retrieved beans.
    public Bean GetBean()
    {
        Bean bean = this.currentlyAvailableBeans.Dequeue();
        return bean;
    }

    // Store bean back into beans (pool)
    public void DisposeBean(Bean bean)
    {
        bean.gameObject.SetActive(false);
        this.player.RemoveBean();
        this.beans.Enqueue(bean);
    }
}
