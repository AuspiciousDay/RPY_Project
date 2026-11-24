using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 스폰 위치")]
    [SerializeField] private TilemapRenderer backgroundRenderer;
    [SerializeField] private float spawnPosition = 1.2f;

    public Monster[] monsterPrefabs;
    public MonsterData[] monsterDatas;

    private readonly List<Monster> monsters = new List<Monster>();
    private Camera mainCam;

    private float spawnInterval = 5f;
    private float timer = 0f;
    public UiManager uiManager;

    private void Awake()
    {
        mainCam = Camera.main;
        uiManager = FindFirstObjectByType<UiManager>();
    }

    private void Update()
    {
        monsters.RemoveAll(monster => monster == null);

        timer += Time.deltaTime;

        if (timer >= spawnInterval || monsters.Count == 0)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    int wave = 3;
    private void SpawnWave()
    {
        wave++;
        int count = Mathf.RoundToInt(wave * 1.5f);
        for (int i = 0; i < count; ++i)
        {
            CreateMonster();
        }
    }

    private void CreateMonster()
    {
        Vector2 spawnPos = GetRandomOutOfScreenPosition();

        // int randomValue = Random.Range(0, 100); //몬스터 스폰 확률 용
        int selectedIndex = Random.Range(0, 2);

        Monster monsterPrefabSpawn = monsterPrefabs[selectedIndex];
        MonsterData data = monsterDatas[selectedIndex];

        Monster monster = Instantiate(monsterPrefabSpawn, spawnPos, Quaternion.identity);

        monster.Setup(data, uiManager);
        monsters.Add(monster);
    }

    private Vector2 GetRandomOutOfScreenPosition()
    {
        if (backgroundRenderer == null) return Vector2.zero;

        Bounds bgBounds = backgroundRenderer.bounds;

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

        if (rects.Count > 0)
        {
            Rect chosen = rects[Random.Range(0, rects.Count)];

            return new Vector2(
                Random.Range(chosen.xMin, chosen.xMax),
                Random.Range(chosen.yMin, chosen.yMax));
        }

        return bgBounds.max;
    }
}
