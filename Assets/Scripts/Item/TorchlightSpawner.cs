using UnityEngine;

public class TorchlightSpawner : MonoBehaviour
{
    public GameObject TorchlightPrefab;

    public float SpawnInterval = 30f;
    public int SpawnCount = 3;

    public SpriteRenderer Ground;
    public LayerMask Obstacles;

    public float spawnPadding = 0.5f;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnTorchlight), 2f, SpawnInterval);
    }

    private void SpawnTorchlight()
    {
        if (Ground == null || mainCam == null || TorchlightPrefab == null) return;

        Bounds bounds = Ground.bounds;

        bounds.Expand(new Vector3(-spawnPadding * 2f, -spawnPadding * 2f, 0f));

        if (bounds.size.x <= 0f || bounds.size.y <= 0f) return;

        int count = 0;

        for (int i = 0; i < 100; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            if (Physics2D.OverlapCircle(spawnPosition, spawnPadding, Obstacles) != null)
            {
                continue;
            }

            Vector3 viewPos = mainCam.WorldToViewportPoint(spawnPosition);

            if (viewPos.x >= -0.1f && viewPos.x <= 1.1f && viewPos.y >= -0.1f && viewPos.y <= 1.1f)
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