using UnityEngine;
using System.Collections.Generic;

public class AnimalManager_Random : MonoBehaviour
{
    public List<GameObject> animalPrefabs;
    public int minAnimals = 10;
    public Vector2 spawnAreaMin = new Vector2(-42f, -26f);
    public Vector2 spawnAreaMax = new Vector2(44.5f, 11.6f);
    public float spawnInterval = 5f; 

    private List<GameObject> activeAnimals = new List<GameObject>();
    private List<GameObject> animalPool = new List<GameObject>();
    private float spawnTimer = 0f;
    void Start()
    {
        for (int i = 0; i < minAnimals; i++)
        {
            GameObject newAnimal = Instantiate(GetRandomPrefab(), Vector2.zero, Quaternion.identity);
            newAnimal.SetActive(false);
            animalPool.Add(newAnimal);

            AnimalTracker tracker = newAnimal.AddComponent<AnimalTracker>();
            tracker.manager = this;

            animalMovement movement = newAnimal.GetComponent<animalMovement>();
            if (movement != null)
            {
                movement.manager = this;
            }
        }

        while (activeAnimals.Count < minAnimals)
        {
            SpawnFromPool();
        }
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activeAnimals.Count < minAnimals)
        {
            SpawnFromPool();
            spawnTimer = 0f;
        }
    }

    private GameObject GetRandomPrefab()
    {
        if (animalPrefabs.Count == 0) return null;
        int randomIndex = Random.Range(0, animalPrefabs.Count);
        return animalPrefabs[randomIndex];
    }

    private void SpawnFromPool()
    {
        if (animalPool.Count == 0) return;

        GameObject animal = animalPool[0];
        animalPool.RemoveAt(0);

        // Spawn tại vị trí cố định (-30, -9, 0)
        Vector2 spawnPosition = new Vector2(-30f, -9f);
        animal.transform.position = spawnPosition;
        animal.SetActive(true);

        activeAnimals.Add(animal);
    }

    public void OnAnimalDied(GameObject animal)
    {
        if (activeAnimals.Contains(animal))
        {
            activeAnimals.Remove(animal);
            animal.SetActive(false);
            animalPool.Add(animal);
        }
    }
}

public class AnimalTracker : MonoBehaviour
{
    public AnimalManager_Random manager;
    public float health = 100f;

    void Update()
    {
        if (health <= 0)
        {
            manager.OnAnimalDied(gameObject);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
