using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 스폰 위치")]
    [SerializeField]private SpriteRenderer backgroundRenderer;
    [SerializeField]private float spawnPosition = 1.2f;

    public Monster[] monsterPrefabs;
    public MonsterData[] monsterDatas;

    private readonly List<Monster> monsters = new List<Monster>();
    private Camera mainCam;

    private  float spawnInterval = 3.5f;
    private float timer = 0f;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if ((timer >= spawnInterval || monsters.Count == 0) && monsters.Count <= 100)
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
            if(monsters.Count >= 100) break;
            CreateMonster();
        }
    }

    private void CreateMonster()
    {
        Vector2 spawnPos = GetRandomOutOfScreenPosition();

        // int randomValue = Random.Range(0, 100); //몬스터 스폰 확률 용
        int selectedIndex = 0;

        Monster monsterPrefabSpawn = monsterPrefabs[selectedIndex];
        MonsterData data = monsterDatas[selectedIndex];

        Monster monster = Instantiate(monsterPrefabSpawn, spawnPos, Quaternion.identity);

        monster.Setup(data);
        monsters.Add(monster);
    }

    private void RemoveMonster(Monster deadMonster)
    {
        if (monsters.Contains(deadMonster))
        {
            monsters.Remove(deadMonster);
        }

        if(deadMonster != null)
        {
            Destroy(deadMonster.gameObject);
        }
    }

    private Vector2 GetRandomOutOfScreenPosition()
    {
        if(backgroundRenderer == null) return Vector2.zero;

        Bounds bgBounds = backgroundRenderer.bounds;
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector2 camPos = mainCam.transform.position;

        float viewMinX = camPos.x - camWidth - spawnPosition;
        float viewMaxX = camPos.x + camWidth + spawnPosition;
        float viewMinY = camPos.y - camHeight - spawnPosition;
        float viewMaxY = camPos.y + camHeight + spawnPosition;

        List<Rect> rects = new List<Rect>();

        if(bgBounds.max.y > viewMaxY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, viewMaxY, bgBounds.max.x, bgBounds.max.y));
        }
        if(bgBounds.min.y < viewMinY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, bgBounds.min.y, bgBounds.max.x, viewMinY));
        }
        if(bgBounds.max.x > viewMaxX)
        {
            rects.Add(Rect.MinMaxRect(viewMaxX, Mathf.Max(bgBounds.min.y, viewMinY), bgBounds.max.x, Mathf.Min(bgBounds.max.y, viewMaxY)));
        }
        if(bgBounds.min.x < viewMinX)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, Mathf.Max(bgBounds.min.y, viewMinY), viewMinX, Mathf.Min(bgBounds.max.y, viewMaxY)));
        }

        if(rects.Count > 0)
        {
            Rect chosen = rects[Random.Range(0, rects.Count)];
            return new Vector2(Random.Range(chosen.xMin, chosen.xMax), Random.Range(chosen.yMin, chosen.yMax));
        }
    
        return bgBounds.max;
    }
}
