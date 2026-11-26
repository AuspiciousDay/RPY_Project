using UnityEngine;
using System.Collections.Generic;

public class HolyWaterWeapon : MonoBehaviour
{
    public float damage = 10f;
    public float attackInterval = 4f;
    public float duration = 3f;
    public float throwRange = 5f;
    public float radius = 1.5f;
    public int attackCount = 1;

    [SerializeField] private HolyWaterProjectile waterPrefab;
    [SerializeField] private HolyWaterAbility areaPrefab;

    private float timer;

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

            water.Init(targetPosition, areaPrefab, damage, radius, duration);
        }
    }
    private List<Monster> FindTargets()
    {
        List<Monster> targets = new List<Monster>();

        Monster[] monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        foreach (Monster monster in monsters)
        {
            float distance = Vector2.Distance(transform.position, monster.transform.position);

            if (distance <= throwRange)
            {
                targets.Add(monster);
            }
        }

        return targets;
    }
}