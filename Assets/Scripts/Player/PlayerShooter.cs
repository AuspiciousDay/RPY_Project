using System.Collections;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public float ProjectileSpeedMultiplier = 1f;
    public int PenetrateCount = 1;
    public float AttackDamage = 25;
    public GameObject AbilityPrefab;

    public float AttackInterval = 1f;
    public int AttackCount = 1;
    private float AttackTimer = 0f;

    private Transform Target;
    private float Distance;

    public bool IsEvolved { get; private set; }

    [SerializeField] private float evolvedInterval = 0.05f;
    [SerializeField] private GameObject evolvedPrefab;

    private PassiveAddDamage damagePassive;

    private void Awake()
    {
        damagePassive = GetComponentInParent<PassiveAddDamage>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (IsEvolved)
        {
            EvolvedAttack();
            return;
        }

        AttackTimer += Time.deltaTime;

        if (AttackTimer >= AttackInterval)
        {
            Target = FindNearestTarget();

            if (Target != null)
            {
                Shoot(Target);
                AttackTimer = 0f;
            }
        }
    }

    private Transform FindNearestTarget()
    {
        Monster[] Monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        Transform NearestTarget = null;
        float NearestDistance = 20f;

        foreach (Monster Monster in Monsters)
        {
            if (Monster.health <= 0) continue;

            Distance = Vector2.Distance(transform.position, Monster.transform.position);

            if (Distance < NearestDistance)
            {
                NearestDistance = Distance;
                NearestTarget = Monster.transform;
            }
        }

        Torchlight[] Torchlights = FindObjectsByType<Torchlight>(FindObjectsSortMode.None);

        foreach (Torchlight Torchlight in Torchlights)
        {
            if (Torchlight.Health <= 0) continue;

            Distance = Vector2.Distance(transform.position, Torchlight.transform.position);

            if (Distance < NearestDistance)
            {
                NearestDistance = Distance;
                NearestTarget = Torchlight.transform;
            }
        }

        return NearestTarget;
    }

    private void Shoot(Transform Target)
    {
        StartCoroutine(ShootCoroutine(Target));
    }

    private IEnumerator ShootCoroutine(Transform Target)
    {
        for (int i = 0; i < AttackCount; i++)
        {
            if (Target == null)
            {
                Target = FindNearestTarget();
            }

            if (Target == null) yield break;

            CreateProjectile(Target);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CreateProjectile(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        GameObject bullet = Instantiate(AbilityPrefab, transform.position, Quaternion.identity);

        PlayerAbility ability = bullet.GetComponent<PlayerAbility>();

        ability.maxCount = PenetrateCount;
        ability.damage = AttackDamage * (damagePassive != null ? damagePassive.Multiplier : 1f);
        ability.speed *= ProjectileSpeedMultiplier;

        ability.SetDirection(direction);
    }

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedPrefab != null)
        {
            AbilityPrefab = evolvedPrefab;
        }

        StopAllCoroutines();
        AttackTimer = 0f;
    }

    private void EvolvedAttack()
    {
        AttackTimer += Time.deltaTime;

        if (AttackTimer < evolvedInterval) return;

        Target = FindNearestTarget();

        if (Target == null) return;

        for (int i = 0; i < AttackCount; i++)
        {
            CreateProjectile(Target);
        }

        AttackTimer = 0f;
    }

    public void MultiplyCooldown(float multiplier)
    {
        AttackInterval = Mathf.Max(0.01f, AttackInterval * multiplier);
        evolvedInterval = Mathf.Max(0.01f, evolvedInterval * multiplier);
    }
}