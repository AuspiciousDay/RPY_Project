
using System.Collections;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public int PenetrateCount = 1;
    public int AttackDamage = 10;
    public GameObject AbilityPrefab;

    public float AttackInterval = 1f;
    public int AttackCount = 1;
    private float AttackTimer = 0f;
    private Monster TargetMonster;
    private float Distance;

    private void Update()
    {
        AttackTimer += Time.deltaTime;

        if (AttackTimer >= AttackInterval)
        {
            TargetMonster = FindNearestMonster();

            if (TargetMonster != null)
            {
                Shoot(TargetMonster);
                AttackTimer = 0f;
            }
        }
    }

    private Monster FindNearestMonster()
    {
        Monster[] Monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        Monster NearestMonster = null;
        float NearestDistance = 20f;

        foreach (Monster Monster in Monsters)
        {
            Distance = Vector2.Distance(
                transform.position,
                Monster.transform.position
            );

            if (Distance < NearestDistance)
            {
                NearestDistance = Distance;
                NearestMonster = Monster;
            }
        }

        return NearestMonster;
    }

    private void Shoot(Monster Target)
    {
        StartCoroutine(ShootCoroutine(Target));
    }

    private IEnumerator ShootCoroutine(Monster Target)
    {
        for (int i = 0; i < AttackCount; i++)
        {
            if (Target == null)
            {
                Target = FindNearestMonster();
            }

            if (Target == null)
            {
                yield break;
            }

            Vector2 direction =
                (Target.transform.position - transform.position).normalized;

            GameObject bullet = Instantiate(
                AbilityPrefab, transform.position, Quaternion.identity);

            PlayerAbility ability = bullet.GetComponent<PlayerAbility>();

            ability.maxCount = PenetrateCount;
            ability.damage = AttackDamage;

            ability.SetDirection(direction);

            yield return new WaitForSeconds(0.1f);
        }
    }

}
