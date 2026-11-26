
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Garlic : MonoBehaviour
{
    public int Damage = 10;
    public float DamageInterval = 1f;

    private CircleCollider2D rangeCollider;
    private LineRenderer lineRenderer;
    private PassiveAddDamage damagePassive;

    private void Awake()
    {
        rangeCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        damagePassive = GetComponentInParent<PassiveAddDamage>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;

        DrawCircle();
    }

    private void DrawCircle()
    {
        int segments = 64;

        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            float x = Mathf.Cos(angle) * rangeCollider.radius;
            float y = Mathf.Sin(angle) * rangeCollider.radius;

            lineRenderer.SetPosition(
                i,
                new Vector3(
                    x + rangeCollider.offset.x,
                    y + rangeCollider.offset.y,
                    0
                )
            );
        }
    }

    public void IncreaseRange(float amount)
    {
        rangeCollider.radius += amount;
        DrawCircle();

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
            monster.TakeDamage(Damage * (damagePassive?.Multiplier ?? 1f));

            Debug.Log("Garlic 지속 공격!");

            yield return new WaitForSeconds(DamageInterval);
        }
    }
}
