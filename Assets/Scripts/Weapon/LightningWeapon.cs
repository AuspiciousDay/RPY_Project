using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    public int damage = 10;
    public int maxCount = 2;

    public float attackInterval = 5f;
    private float lifeTime = 0.3f;

    public GameObject lightningPrefab;

    private Camera mainCam;
    private float timer;
    private PassiveAddDamage damagePassive;

    private void Awake()
    {
        mainCam = Camera.main;
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

    public void Attack()
    {
        List<Monster> targets = FindTargets();

        int count = Mathf.Min(maxCount, targets.Count);
        float effectiveDamage = damage * (damagePassive?.Multiplier ?? 1f);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, targets.Count);

            Monster target = targets[index];

            targets.RemoveAt(index);

            if (target == null) continue;

            GameObject lightning = Instantiate(lightningPrefab, target.transform.position, Quaternion.identity);

            target.TakeDamage(effectiveDamage);

            Destroy(lightning, lifeTime);
        }
    }
}
