using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private TilemapRenderer backgroundRenderer;
    [SerializeField] private Tilemap backgroundTilemap;
    private float spawnPosition = 1.2f;
    private float spawnPadding = 0.5f;

    public Monster[] monsterPrefabs;
    public MonsterData[] monsterDatas;
 
    private float eliteSpawnChance = 0.15f; 
    private float bossSpawnTime = 300f;

    private readonly List<Monster> monsters = new List<Monster>();
    private Camera mainCam;

    private float spawnInterval = 5f;
    private float timer = 0f;
    private float playTime = 0f; 
    private bool hasSpawnedBoss = false;

    public UiManager uiManager;

    private int wave = 3;

    private readonly List<int> normalIndex = new List<int>();
    private readonly List<int> eliteIndex = new List<int>();
    private int bossIndex = -1;

    private void Awake()
    {
        mainCam = Camera.main;
        uiManager = FindFirstObjectByType<UiManager>();

        if (backgroundTilemap == null && backgroundRenderer != null)
        {
            backgroundTilemap = backgroundRenderer.GetComponent<Tilemap>();
        }

        CategorizeMonsters();
    }

    // 스크립트 타입 보고 알아서 판단
    private void CategorizeMonsters()
    {
        normalIndex.Clear();
        eliteIndex.Clear();
        bossIndex = -1;

        int count = Mathf.Min(monsterPrefabs.Length, monsterDatas.Length);
        for (int i = 0; i < count; i++)
        {
            if (monsterPrefabs[i] is Boss)
            {
                bossIndex = i;
            }
            else if (monsterPrefabs[i] is EliteMonster)
            {
                eliteIndex.Add(i);
            }
            else
            {
                normalIndex.Add(i);
            }
        }
    }

    private bool hasStarted = false;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (!hasStarted)
        {
            hasStarted = true;
            SpawnWave();
        }

        monsters.RemoveAll(monster => monster == null);

        playTime += Time.deltaTime;
        if (!hasSpawnedBoss && playTime >= bossSpawnTime)
        {
            if (SpawnBoss())
            {
                hasSpawnedBoss = true;
            }
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    private void SpawnWave()
    {
        int nextWave = wave++;
        int count = Mathf.RoundToInt(nextWave * 1.5f);
        bool spawned = false;

        for (int i = 0; i < count; ++i)
        {
            if (CreateMonster())
            {
                spawned = true;
            }
        }

        if (spawned)
        {
            wave = nextWave;
        }
    }

    private bool CreateMonster()
    {
        Vector2? spawnPos = GetRandomOutOfScreenPosition();
        if (!spawnPos.HasValue) return false;

        int selectedIndex = -1;

        if (eliteIndex.Count > 0 && Random.value < eliteSpawnChance)
        {
            selectedIndex = eliteIndex[Random.Range(0, eliteIndex.Count)];
        }
        else if (normalIndex.Count > 0)
        {
            selectedIndex = normalIndex[Random.Range(0, normalIndex.Count)];
        }

        if (selectedIndex == -1) return false;

        Monster monster = Instantiate(monsterPrefabs[selectedIndex], spawnPos.Value, Quaternion.identity);
        monster.Setup(monsterDatas[selectedIndex], uiManager);
        monsters.Add(monster);

        return true;
    }

    private bool SpawnBoss()
    {
        if (bossIndex == -1) return false;

        Vector2? spawnPos = GetRandomOutOfScreenPosition();
        if (!spawnPos.HasValue) return false;

        Monster boss = Instantiate(monsterPrefabs[bossIndex], spawnPos.Value, Quaternion.identity);
        boss.Setup(monsterDatas[bossIndex], uiManager);
        monsters.Add(boss);

        return true;
    }

    private Vector2? GetRandomOutOfScreenPosition()
    {
        if (backgroundRenderer == null || backgroundTilemap == null || mainCam == null)
        {
            return null;
        }

        Bounds bgBounds = backgroundRenderer.bounds;

        bgBounds.Expand(new Vector3(-spawnPadding * 2f, -spawnPadding * 2f, 0f));

        if (bgBounds.size.x <= 0 || bgBounds.size.y <= 0)
        {
            return null;
        }

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector2 camPos = mainCam.transform.position;

        float viewMinX = camPos.x - camWidth - spawnPosition;
        float viewMaxX = camPos.x + camWidth + spawnPosition;
        float viewMinY = camPos.y - camHeight - spawnPosition;
        float viewMaxY = camPos.y + camHeight + spawnPosition;

        viewMinX = Mathf.Clamp(viewMinX, bgBounds.min.x, bgBounds.max.x);
        viewMaxX = Mathf.Clamp(viewMaxX, bgBounds.min.x, bgBounds.max.x);
        viewMinY = Mathf.Clamp(viewMinY, bgBounds.min.y, bgBounds.max.y);
        viewMaxY = Mathf.Clamp(viewMaxY, bgBounds.min.y, bgBounds.max.y);

        List<Rect> rects = new List<Rect>();
        if (bgBounds.max.y > viewMaxY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, viewMaxY, bgBounds.max.x, bgBounds.max.y));
        }
        if (bgBounds.min.y < viewMinY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, bgBounds.min.y, bgBounds.max.x, viewMinY));
        }


        if (bgBounds.max.x > viewMaxX && viewMaxY > viewMinY)
        {
            rects.Add(Rect.MinMaxRect(viewMaxX, viewMinY, bgBounds.max.x, viewMaxY));
        }


        if (bgBounds.min.x < viewMinX && viewMaxY > viewMinY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, viewMinY, viewMinX, viewMaxY));
        }

        if (rects.Count == 0) return null;

        for (int i = 0; i < 30; i++)
        {
            Rect chosen = rects[Random.Range(0, rects.Count)];

            Vector2 position = new Vector2(Random.Range(chosen.xMin, chosen.xMax), Random.Range(chosen.yMin, chosen.yMax));

            if (IsValidSpawnPosition(position))
            {
                return position;
            }
        }

        return null;
    }

    private bool IsValidSpawnPosition(Vector2 position)
    {
        if (!backgroundTilemap.HasTile(
            backgroundTilemap.WorldToCell(position)))
        {
            return false;
        }

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                Vector2 checkPosition = position + new Vector2(x * spawnPadding, y * spawnPadding);

                Vector3Int cell =
                    backgroundTilemap.WorldToCell(checkPosition);

                if (!backgroundTilemap.HasTile(cell))
                {
                    return false;
                }
            }
        }

        return true;
    }
}