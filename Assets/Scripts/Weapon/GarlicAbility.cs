using UnityEngine;
using System.Collections.Generic;

public class GarlicAbility : MonoBehaviour
{
    public float Damage = 10;
    public float DamageInterval = 1f;

    public bool IsEvolved { get; private set; }

    [SerializeField] private float evolvedRangeMultiplier = 2f;
    [SerializeField] private float slowMultiplier = 0.5f;

    private CircleCollider2D rangeCollider;
    private float timer;

    private readonly HashSet<Monster> hitMonsters = new HashSet<Monster>();
    private readonly HashSet<Monster> slowedMonsters = new HashSet<Monster>();

    [SerializeField] private Transform garlicVisual;

    private float originalRadius;
    private Vector3 originalScale;

    [SerializeField] private GameObject evolvedPrefab;

    private void Awake()
    {
        rangeCollider = GetComponent<CircleCollider2D>();

        originalRadius = rangeCollider.radius;
        originalScale = garlicVisual.localScale;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        DetectMonsters();

        timer += Time.deltaTime;

        if (timer >= DamageInterval)
        {
            Attack();
            timer = 0f;
        }
    }

    public void IncreaseRange(float amount)
    {
        rangeCollider.radius += amount;

        UpdateVisualScale();
    }

    private void UpdateVisualScale()
    {
        float ratio = rangeCollider.radius / originalRadius;

        garlicVisual.localScale = originalScale * ratio;
    }

    private void DetectMonsters()
    {
        Vector2 center = transform.TransformPoint(rangeCollider.offset);
        float radius = rangeCollider.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        hitMonsters.Clear();

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponentInParent<Monster>();

            if (monster != null)
            {
                hitMonsters.Add(monster);
            }
        }

        if (IsEvolved)
        {
            UpdateSlow();
        }
    }

    private void Attack()
    {
        foreach (Monster monster in hitMonsters)
        {
            if (monster != null)
            {
                monster.TakeDamage(Damage);
            }
        }
    }

    private void UpdateSlow()
    {
        foreach (Monster monster in slowedMonsters)
        {
            if (monster != null && !hitMonsters.Contains(monster))
            {
                monster.RemoveSlow();
            }
        }

        slowedMonsters.RemoveWhere(monster => monster == null || !hitMonsters.Contains(monster));

        foreach (Monster monster in hitMonsters)
        {
            if (slowedMonsters.Add(monster))
            {
                monster.ApplySlow(slowMultiplier);
            }
        }
    }

    private void OnDisable()
    {
        foreach (Monster monster in slowedMonsters)
        {
            if (monster != null)
            {
                monster.RemoveSlow();
            }
        }

        slowedMonsters.Clear();
        hitMonsters.Clear();
    }

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedPrefab != null)
        {
            garlicVisual.gameObject.SetActive(false);
            Destroy(garlicVisual.gameObject);

            GameObject visual = Instantiate(evolvedPrefab, transform);

            garlicVisual = visual.transform;
            garlicVisual.localPosition = Vector3.zero;
            garlicVisual.localRotation = Quaternion.identity;

            originalScale = garlicVisual.localScale;
        }

        rangeCollider.radius *= evolvedRangeMultiplier;

        UpdateVisualScale();
    }
}