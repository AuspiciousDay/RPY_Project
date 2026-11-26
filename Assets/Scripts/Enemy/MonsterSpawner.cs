using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] public LayerMask Obstacles;

    [Header("뼈 감옥")]
    public CageSkeleton cageMonsterPrefab;
    public MonsterData cageMonsterData;
    public float cageCount = 24f;
    public float cageRadius = 7f;
    public float cageDuration = 50f;
    [SerializeField, Min(0f)] private float cageShrinkSpeed;
    public float eilteEventInterval = 50f;
    public int cageInnerNormalCount = 8;
    public float minMonsterDistanceFromPlayer = 3f;

    private float eliteEventTimer = 0f;

    private float spawnPosition = 1.2f;
    private float spawnPadding = 0.5f;

    public Monster[] monsterPrefabs;
    public MonsterData[] monsterDatas;

    private float bossSpawnTime = 300f;
    [SerializeField, Min(0.01f)] private float bossHealthMultiplier = 1f;
    [SerializeField, Min(0.01f)] private float bossSpeedMultiplier = 1f;
    [SerializeField, Min(0f)] private float healthGrowthPerStep;
    private const float HealthGrowthInterval = 20f;
    private int healthGrowthSteps;

    private readonly List<Monster> monsters = new List<Monster>();

    private Camera mainCam;
    private Transform playerTransform;

    private float spawnInterval = 5f;
    private float timer = 0f;
    private float playTime = 0f;

    private bool hasSpawnedBoss = false;
    private float nextBossSpawnAttempt;

    public UiManager uiManager;

    private int wave = 3;
    [SerializeField, Min(0)] private int maxMonstersPerWave;

    private readonly List<int> normalIndex = new List<int>();
    private readonly List<int> eliteIndex = new List<int>();

    private int bossIndex = -1;

    private bool hasStarted = false;


    private void Awake()
    {
        mainCam = Camera.main;
        uiManager = FindFirstObjectByType<UiManager>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        CategorizeMonsters();
    }

    // 몬스터 타입 분류
    private void CategorizeMonsters()
    {
        normalIndex.Clear();
        eliteIndex.Clear();
        bossIndex = -1;

        int count = Mathf.Min(monsterPrefabs.Length, monsterDatas.Length);

        for (int i = 0; i < count; i++)
        {
            if (monsterPrefabs[i] == null || monsterDatas[i] == null) continue;

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

    private void Update()
    {
        if (hasSpawnedBoss) return;
        if (Time.timeScale == 0f) return;

        if (!hasStarted)
        {
            hasStarted = true;
            SpawnWave();
        }

        monsters.RemoveAll(monster => monster == null);

        playTime += Time.deltaTime;
        int elapsedSteps = Mathf.FloorToInt(playTime / HealthGrowthInterval);
        if (healthGrowthPerStep > 0f && elapsedSteps > healthGrowthSteps)
        {
            float addedHealthFraction = healthGrowthPerStep * (elapsedSteps - healthGrowthSteps);
            foreach (Monster monster in monsters)
                monster.AddBaseHealthFraction(addedHealthFraction);

            healthGrowthSteps = elapsedSteps;
        }

        // 보스 생성
        if (bossIndex >= 0 && playTime >= bossSpawnTime && playTime >= nextBossSpawnAttempt)
        {
            hasSpawnedBoss = SpawnBoss();
            if (hasSpawnedBoss) return;
            nextBossSpawnAttempt = playTime + 1f;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnWave();
            timer = 0f;
        }

        // 뼈 감옥 이벤트
        eliteEventTimer += Time.deltaTime;

        if (eliteEventTimer >= eilteEventInterval)
        {
            SpawnEliteCageEvent();
            eliteEventTimer = 0f;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnEliteCageEvent();
        }
#endif
    }

    private void SpawnWave()
    {
        int count = Mathf.RoundToInt(wave * 1.5f);
        if (maxMonstersPerWave > 0) count = Mathf.Min(count, maxMonstersPerWave);
        bool spawned = false;

        for (int i = 0; i < count; i++)
        {
            if (CreateMonster())
            {
                spawned = true;
            }
        }

        if (spawned)
        {
            wave++;
        }
    }

    private bool CreateMonster()
    {
        if (normalIndex.Count == 0) return false;

        Vector2? spawnPos = GetRandomOutOfScreenPosition();

        if (!spawnPos.HasValue) return false;

        int selectedIndex = normalIndex[Random.Range(0, normalIndex.Count)];

        Monster monster = Instantiate(monsterPrefabs[selectedIndex], spawnPos.Value, Quaternion.identity);

        SetupWaveMonster(monster, monsterDatas[selectedIndex]);

        monsters.Add(monster);

        return true;
    }

    // 뼈 감옥 이벤트
    private void SpawnEliteCageEvent()
    {
        if (playerTransform == null || eliteIndex.Count == 0) return;

        Vector2 centerPos = playerTransform.position;

        // 원형 뼈 감옥 생성
        if (cageMonsterPrefab != null && cageMonsterData != null && backgroundRenderer != null)
        {
            int count = Mathf.RoundToInt(cageCount);
            Bounds mapBounds = backgroundRenderer.bounds;
            mapBounds.Expand(new Vector3(-2f * spawnPadding, -2f * spawnPadding));
            var occupiedPositions = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2f / count;

                Vector2 spawnPos = centerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * cageRadius;

                spawnPos.x = Mathf.Clamp(spawnPos.x, mapBounds.min.x, mapBounds.max.x);
                spawnPos.y = Mathf.Clamp(spawnPos.y, mapBounds.min.y, mapBounds.max.y);
                if (Vector2.Distance(centerPos, spawnPos) < minMonsterDistanceFromPlayer ||
                    !IsValidSpawnPosition(spawnPos) ||
                    occupiedPositions.Exists(position => (position - spawnPos).sqrMagnitude < 1f))
                    continue;

                CageSkeleton cageMob = Instantiate(cageMonsterPrefab, spawnPos, Quaternion.identity);

                cageMob.Initialize(centerPos, cageDuration, cageShrinkSpeed);
                SetupWaveMonster(cageMob, cageMonsterData);
                monsters.Add(cageMob);
                occupiedPositions.Add(spawnPos);
            }
        }

        // 엘리트 몬스터 1마리 생성
        int selectedElite = eliteIndex[Random.Range(0, eliteIndex.Count)];

        Vector2? elitePos = GetValidInnerCagePosition(centerPos, cageRadius * 0.75f, minMonsterDistanceFromPlayer);

        if (elitePos.HasValue)
        {
            Monster elite = Instantiate(monsterPrefabs[selectedElite], elitePos.Value, Quaternion.identity);

            SetupWaveMonster(elite, monsterDatas[selectedElite]);

            monsters.Add(elite);
        }

        // 감옥 내부 일반 몬스터 생성
        if (normalIndex.Count > 0)
        {
            for (int i = 0; i < cageInnerNormalCount; i++)
            {
                int selectedNormal = normalIndex[Random.Range(0, normalIndex.Count)];

                Vector2? normalPos = GetValidInnerCagePosition(centerPos, cageRadius * 0.8f, minMonsterDistanceFromPlayer);

                if (!normalPos.HasValue) continue;

                Monster normal = Instantiate(monsterPrefabs[selectedNormal], normalPos.Value, Quaternion.identity);

                SetupWaveMonster(normal, monsterDatas[selectedNormal]);

                monsters.Add(normal);
            }
        }
    }

    // 감옥 내부의 유효한 생성 위치 탐색
    private Vector2? GetValidInnerCagePosition(Vector2 center, float maxRadius, float minDistanceFromPlayer)
    {
        if (playerTransform == null) return null;

        for (int i = 0; i < 25; i++)
        {
            Vector2 candidate = center + Random.insideUnitCircle * maxRadius;

            if (!IsValidSpawnPosition(candidate)) continue;

            if (Vector2.Distance(candidate, playerTransform.position) >= minDistanceFromPlayer)
            {
                return candidate;
            }
        }

        return null;
    }

    // 보스 생성
    private void SetupWaveMonster(Monster monster, MonsterData data)
    {
        monster.Setup(data, uiManager);
        monster.AddBaseHealthFraction(healthGrowthPerStep * healthGrowthSteps);
    }

    private bool SpawnBoss()
    {
        if (bossIndex == -1) return false;

        Vector2? spawnPos = GetRandomOutOfScreenPosition() ?? GetFallbackBossPosition();

        if (!spawnPos.HasValue) return false;

        Monster boss = Instantiate(monsterPrefabs[bossIndex], spawnPos.Value, Quaternion.identity);

        boss.Setup(monsterDatas[bossIndex], uiManager);
        boss.health *= bossHealthMultiplier;
        boss.moveSpeed *= bossSpeedMultiplier;

        foreach (Monster monster in monsters)
        {
            if (monster == null) continue;
            monster.gameObject.SetActive(false);
            Destroy(monster.gameObject);
        }
        monsters.Clear();
        monsters.Add(boss);

        return true;
    }

    private Vector2? GetFallbackBossPosition()
    {
        if (backgroundRenderer == null || playerTransform == null) return null;

        Bounds bounds = backgroundRenderer.bounds;
        bounds.Expand(new Vector3(-2f * spawnPadding, -2f * spawnPadding));
        if (bounds.size.x <= 0f || bounds.size.y <= 0f) return null;

        Vector2 playerPosition = playerTransform.position;
        Vector2? best = null;
        float bestDistanceSquared = minMonsterDistanceFromPlayer * minMonsterDistanceFromPlayer;
        const int gridSteps = 16;
        for (int x = 0; x <= gridSteps; x++)
        {
            for (int y = 0; y <= gridSteps; y++)
            {
                var position = new Vector2(
                    Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / gridSteps),
                    Mathf.Lerp(bounds.min.y, bounds.max.y, (float)y / gridSteps));
                float distanceSquared = (position - playerPosition).sqrMagnitude;
                if (distanceSquared <= bestDistanceSquared || !IsValidSpawnPosition(position)) continue;
                best = position;
                bestDistanceSquared = distanceSquared;
            }
        }

        return best;
    }

    // 카메라 화면 밖 생성 위치 탐색
    private Vector2? GetRandomOutOfScreenPosition()
    {
        if (backgroundRenderer == null || mainCam == null) return null;

        Bounds bgBounds = backgroundRenderer.bounds;

        bgBounds.Expand(new Vector3(-spawnPadding * 2f, -spawnPadding * 2f, 0f));

        if (bgBounds.size.x <= 0f || bgBounds.size.y <= 0f) return null;

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

        // 화면 위쪽
        if (bgBounds.max.y > viewMaxY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, viewMaxY, bgBounds.max.x, bgBounds.max.y));
        }

        // 화면 아래쪽
        if (bgBounds.min.y < viewMinY)
        {
            rects.Add(Rect.MinMaxRect(bgBounds.min.x, bgBounds.min.y, bgBounds.max.x, viewMinY));
        }

        // 화면 오른쪽
        if (bgBounds.max.x > viewMaxX && viewMaxY > viewMinY)
        {
            rects.Add(Rect.MinMaxRect(viewMaxX, viewMinY, bgBounds.max.x, viewMaxY));
        }

        // 화면 왼쪽
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

    // 맵 범위 및 장애물 검사
    private bool IsValidSpawnPosition(Vector2 position)
    {
        if (backgroundRenderer == null) return false;

        Bounds bounds = backgroundRenderer.bounds;

        if (position.x < bounds.min.x + spawnPadding || position.x > bounds.max.x - spawnPadding || position.y < bounds.min.y + spawnPadding || position.y > bounds.max.y - spawnPadding)
        {
            return false;
        }

        return Physics2D.OverlapCircle(position, spawnPadding, Obstacles) == null;
    }
}
