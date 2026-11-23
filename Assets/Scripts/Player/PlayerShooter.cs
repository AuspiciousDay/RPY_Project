
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject AbilityPrefab;

    public float AttackInterval = 1f;

    private float AttackTimer = 0f;

    private void Update()
    {
        AttackTimer += Time.deltaTime;

        if (AttackTimer >= AttackInterval)
        {
            Monster Target = FindNearestMonster();

            if (Target != null)
            {
                Shoot(Target);
                AttackTimer = 0f;
            }
        }
    }

    private Monster FindNearestMonster()
    {
        Monster[] Monsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        Monster NearestMonster = null;
        float NearestDistance = Mathf.Infinity;

        foreach (Monster Monster in Monsters)
        {
            float Distance = Vector2.Distance(
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
        Vector2 Direction = (Target.transform.position - transform.position).normalized;    // 몬스터가 있는 방향 한번 계산

        GameObject Ability = Instantiate(AbilityPrefab, transform.position, Quaternion.identity);   // 투사체


        Ability.GetComponent<PlayerAbility>().SetDirection(Direction);
    }
}
