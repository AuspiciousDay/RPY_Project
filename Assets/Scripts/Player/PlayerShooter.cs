
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject AbilityPrefab;

    public float AttackInterval = 1f;
    public int AttackCount = 1;
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
        GameObject[] bullets = new GameObject[AttackCount];
        for (int i = 0; i < AttackCount; i++)
        {
            bullets[i] = Instantiate(AbilityPrefab, transform.position, Quaternion.identity);
            PlayerAbility ability = bullets[i].GetComponent<PlayerAbility>();
            Vector2 direction = (Target.transform.position - transform.position).normalized;
            ability.SetDirection(direction);
        }
    }

}
