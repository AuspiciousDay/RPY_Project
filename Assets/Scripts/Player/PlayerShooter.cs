
using System.Collections;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public int PenetrateCount = 1;
    public float AttackDamage = 25f;
    public GameObject AbilityPrefab;

    public float AttackInterval = 1f;
    public int AttackCount = 1;
    private float AttackTimer = 0f;
    
    private Transform Target; // 수정
    private float Distance;

    private void Update()
    {
        AttackTimer += Time.deltaTime;

        if (AttackTimer >= AttackInterval)
        {
            Target = FindNearestTarget(); // 수정

            if (Target != null)
            {
                Shoot(Target);
                AttackTimer = 0f;
            }
        }
    }

    // 수정 : 몬스터와 화로 탐색
    private Transform FindNearestTarget()
    {
        Monster[] Monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        Transform NearestTarget = null;
        float NearestDistance = 20f;

        foreach (Monster Monster in Monsters)
        {
            Distance = Vector2.Distance(transform.position, Monster.transform.position);

            if (Distance < NearestDistance)
            {
                NearestDistance = Distance;
                NearestTarget = Monster.transform;
            }
        }

        // 추가 : 화로 탐색
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

    private void Shoot(Transform Target) // 수정
    {
        StartCoroutine(ShootCoroutine(Target));
    }

    private IEnumerator ShootCoroutine(Transform Target) // 수정
    {
        for (int i = 0; i < AttackCount; i++)
        {
            if (Target == null)
            {
                Target = FindNearestTarget(); // 수정
            }

            if (Target == null)
            {
                yield break;
            }

            Vector2 direction = (Target.position - transform.position).normalized;

            GameObject bullet = Instantiate(AbilityPrefab, transform.position, Quaternion.identity);

            PlayerAbility ability = bullet.GetComponent<PlayerAbility>();

            ability.maxCount = PenetrateCount;
            ability.damage = AttackDamage;

            ability.SetDirection(direction);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
