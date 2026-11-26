
using UnityEngine;
using System.Collections;

public class Garlic : MonoBehaviour
{
    public int Damage = 10;
    public float DamageInterval = 1f;

    private CircleCollider2D rangeCollider;

    private void Awake()
    {
        rangeCollider = GetComponent<CircleCollider2D>();
    }

    public void IncreaseRange(float amount)
    {
        rangeCollider.radius += amount;

        Debug.Log("Garlic 범위 증가: " + rangeCollider.radius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform target = collision.transform;

        if (target.CompareTag("Untagged"))
            target = target.root;

        if (target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();

            if (monster != null)
            {
                StartCoroutine(ContinuousDamage(monster));
            }
        }
    }

    private IEnumerator ContinuousDamage(Monster monster)
    {
        while (monster != null)
        {
            monster.TakeDamage(Damage);


            yield return new WaitForSeconds(DamageInterval);
        }
    }
}
