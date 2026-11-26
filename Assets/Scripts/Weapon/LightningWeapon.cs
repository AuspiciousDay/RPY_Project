using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    public float damage = 10f;
    public int maxCount = 2;

    public float attackInterval = 5f;
    private float lifeTime = 0.3f;

    public GameObject lightningPrefab;

    private Camera mainCam;
    private float timer;

    public bool IsEvolved { get; private set; }

    [SerializeField] private LightningArea lightningAreaPrefab;
    [SerializeField] private float areaRadius = 2f;
    [SerializeField] private float areaDuration = 3f;
    [SerializeField] private float areaDamageMultiplier = 0.3f;

    [SerializeField] private GameObject evolvedPrefab;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            Attack();
            timer = 0f;
        }
    }

    private List<Monster> FindTargets()
    {
        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        List<Monster> targets = new List<Monster>();

        foreach (Monster monster in monsters)
        {
            Vector3 viewPos = mainCam.WorldToViewportPoint(monster.transform.position);

            if (viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f && viewPos.z > 0f)
            {
                targets.Add(monster);
            }
        }

        return targets;
    }

    private void Attack()
    {
        List<Monster> targets = FindTargets();

        int count = Mathf.Min(maxCount, targets.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, targets.Count);

            Monster target = targets[index];

            targets.RemoveAt(index);

            if (target == null) continue;

            Vector3 targetPosition = target.transform.position;

            GameObject lightning = Instantiate(lightningPrefab, targetPosition, Quaternion.identity);

            target.TakeDamage(damage);

            Destroy(lightning, lifeTime);

            if (IsEvolved)
            {
                LightningArea area = Instantiate(lightningAreaPrefab, targetPosition, Quaternion.identity);

                area.Init(damage * areaDamageMultiplier, areaRadius, areaDuration);
            }
        }
    }

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedPrefab != null)
        {
            lightningPrefab = evolvedPrefab;
        }
    }
}
