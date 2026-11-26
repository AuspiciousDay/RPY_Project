
using UnityEngine;
using UnityEngine.Tilemaps; // 추가

public class TorchlightSpawner : MonoBehaviour
{
    public GameObject TorchlightPrefab;

    public float SpawnInterval = 10f;
    public int SpawnCount = 5;

    public Vector2 SpawnArea = new Vector2(20f, 20f);

    public Tilemap Ground; 
    public Tilemap Obstacles;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        InvokeRepeating("SpawnTorchlight", 2f, SpawnInterval);
    }

    private void SpawnTorchlight()
    {
        int count = 0;

        for (int i = 0; i < 100; i++)
        {
            float randomX = Random.Range(-SpawnArea.x, SpawnArea.x);
            float randomY = Random.Range(-SpawnArea.y, SpawnArea.y);

            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // 바닥이 없으면 생성하지 않음
            if (!Ground.HasTile(Ground.WorldToCell(spawnPosition)))
            {
                continue;
            }

            // 장애물이 있으면 생성하지 않음
            if (Obstacles.HasTile(Obstacles.WorldToCell(spawnPosition)))
            {
                continue;
            }

            Vector3 viewPos = mainCam.WorldToViewportPoint(spawnPosition);

            if (viewPos.x >= -0.1f && viewPos.x <= 1.1f &&
                viewPos.y >= -0.1f && viewPos.y <= 1.1f)
            {
                continue;
            }

            Instantiate(TorchlightPrefab, spawnPosition, Quaternion.identity);

            count++;

            if (count >= SpawnCount)
            {
                break;
            }
        }
    }
}
