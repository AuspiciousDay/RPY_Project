
using UnityEngine;

public class HealthItemSpawner : MonoBehaviour
{
    public GameObject HealthItemPrefab;

    public float SpawnInterval = 15f;

    public Vector2 SpawnArea = new Vector2(10f, 10f);

    private void Start()
    {
        InvokeRepeating("SpawnHealthItem", 2f, SpawnInterval);
    }

    private void SpawnHealthItem()
    {
        float randomX = Random.Range(-SpawnArea.x, SpawnArea.x);
        float randomY = Random.Range(-SpawnArea.y, SpawnArea.y);

        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

        Instantiate(HealthItemPrefab, spawnPosition, Quaternion.identity);
    }
}
