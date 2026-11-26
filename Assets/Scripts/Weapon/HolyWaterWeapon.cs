using UnityEngine;
using System.Collections.Generic;

public class HolyWaterWeapon : MonoBehaviour
{
    public float ProjectileSpeedMultiplier = 1f;

    public float damage = 10f;
    public float attackInterval = 4f;
    public float duration = 3f;
    public float throwRange = 5f;
    public float radius = 1.5f;
    public int attackCount = 1;

    [SerializeField] private HolyWaterProjectile waterPrefab;
    [SerializeField] private HolyWaterAbility areaPrefab;

    private float timer;

    [SerializeField] private HolyWaterProjectile evolvedWaterPrefab;
    [SerializeField] private HolyWaterAbility evolvedAreaPrefab;

    public bool IsEvolved { get; private set; }

    private PassiveAddDamage damagePassive;

    private void Awake()
    {
        damagePassive = GetComponentInParent<PassiveAddDamage>();
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

    private void Attack()
    {
        float effectiveDamage = damage * (damagePassive != null ? damagePassive.Multiplier : 1f);
        List<Monster> targets = FindTargets();

        for (int i = 0; i < attackCount; i++)
        {
            Vector2 targetPosition;

            if (targets.Count > 0)
            {
                int index = Random.Range(0, targets.Count);

                targetPosition = targets[index].transform.position;

                targets.RemoveAt(index);
            }
            else
            {
                targetPosition = (Vector2)transform.position + Random.insideUnitCircle * throwRange;
            }

            HolyWaterProjectile water = Instantiate(waterPrefab, transform.position, Quaternion.identity);

            water.ApplyVelocity(ProjectileSpeedMultiplier);

            water.Init(targetPosition, areaPrefab, effectiveDamage, radius, duration, IsEvolved);
        }
    }

    private List<Monster> FindTargets()
    {
        List<Monster> targets = new List<Monster>();

        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        foreach (Monster monster in monsters)
        {
            if (monster.health <= 0) continue;

            float distance = Vector2.Distance(transform.position, monster.transform.position);

            if (distance <= throwRange)
            {
                targets.Add(monster);
            }
        }

        return targets;
    }

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedWaterPrefab != null)
        {
            waterPrefab = evolvedWaterPrefab;
        }

        if (evolvedAreaPrefab != null)
        {
            areaPrefab = evolvedAreaPrefab;
        }
    }
}