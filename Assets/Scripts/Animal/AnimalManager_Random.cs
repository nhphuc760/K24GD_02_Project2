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
    private float spawnTimer = 0f;

    void Start()
    {
        // Spawn thêm để đủ minAnimals (10), giả sử map đã có 6 con pre-placed
        while (activeAnimals.Count < minAnimals)
        {
            SpawnAtRandomPosition();
        }
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activeAnimals.Count < minAnimals)
        {
            SpawnAtRandomPosition();
            spawnTimer = 0f;
        }
    }

    private void SpawnAtRandomPosition()
    {
        if (animalPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0, animalPrefabs.Count);
        GameObject selectedPrefab = animalPrefabs[randomIndex];

        // Spawn tại vị trí cố định (60, 6, 0)
        Vector2 spawnPosition = new Vector2(60f, 6f);

        GameObject newAnimal = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        activeAnimals.Add(newAnimal);

        AnimalTracker tracker = newAnimal.AddComponent<AnimalTracker>();
        tracker.manager = this;

        animalMovement movement = newAnimal.GetComponent<animalMovement>();
        if (movement != null)
        {
            movement.manager = this;
        }
    }

    public void OnAnimalDied(GameObject animal)
    {
        if (activeAnimals.Contains(animal))
        {
            activeAnimals.Remove(animal);
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
