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

    private readonly HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    private readonly HashSet<Monster> hitMonsters = new HashSet<Monster>();
    private readonly HashSet<Monster> slowedMonsters = new HashSet<Monster>();

    [SerializeField] private Transform garlicVisual;

    private float originalRadius;
    private Vector3 originalScale;

    [SerializeField] private GameObject evolvedPrefab;

    private bool isInitialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        rangeCollider = GetComponent<CircleCollider2D>();
        originalRadius = rangeCollider.radius;
        originalScale = garlicVisual.localScale;

        isInitialized = true;
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
        Initialize();

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

        hitTargets.Clear();
        hitMonsters.Clear();

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();

            if (target == null) continue;

            hitTargets.Add(target);

            if (target is Monster monster)
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
        foreach (IDamageable target in hitTargets)
        {
            if (target is MonoBehaviour behaviour && behaviour != null)
            {
                target.TakeDamage(Damage);
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