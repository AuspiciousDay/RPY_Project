using System.Collections.Generic;
using UnityEngine;

public class BellWeapon : MonoBehaviour
{
    public float damage = 15f;
    public float attackInterval = 5f;
    public float duration = 3f;
    public float radius = 2f;
    public float rotateSpeed = 180f;
    public int attackCount = 1;

    [SerializeField] private BellAbility bellPrefab;
    [SerializeField] private BellAbility evolvedPrefab;

    private float timer;
    private PassiveAddDamage damagePassive;

    internal float EffectiveDamage => damage * (damagePassive != null ? damagePassive.Multiplier : 1f);

    public bool IsEvolved { get; private set; }

    private readonly List<BellAbility> activeBells = new List<BellAbility>();

    private void Awake()
    {
        damagePassive = GetComponentInParent<PassiveAddDamage>();
    }

    private void Update()
    {
        if (IsEvolved)
        {
            SyncEvolvedBells();
            return;
        }

        if (Time.timeScale == 0f) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            Attack();
            timer = 0f;
        }
    }

    private void Attack()
    {
        activeBells.RemoveAll(bell => bell == null);

        for (int i = 0; i < attackCount; i++)
        {
            float angle = 360f / attackCount * i;

            BellAbility bell = Instantiate(bellPrefab, transform.position, Quaternion.identity);

            bell.Init(transform, angle, radius, rotateSpeed, EffectiveDamage, duration);

            activeBells.Add(bell);
        }
    }

    private void SyncEvolvedBells()
    {
        activeBells.RemoveAll(bell => bell == null);
        int count = Mathf.Max(0, attackCount);
        if (activeBells.Count == count) return;

        float startAngle = activeBells.Count > 0 ? activeBells[0].OrbitAngle : 0f;
        while (activeBells.Count > count)
        {
            int last = activeBells.Count - 1;
            Destroy(activeBells[last].gameObject);
            activeBells.RemoveAt(last);
        }

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + 360f * i / count;
            if (i < activeBells.Count)
            {
                activeBells[i].SetOrbitAngle(angle);
                continue;
            }

            BellAbility bell = Instantiate(bellPrefab, transform.position, Quaternion.identity);
            bell.Init(transform, angle, radius, rotateSpeed, EffectiveDamage, duration, this);
            activeBells.Add(bell);
        }
    }

    public void Evolve()
    {
        if (IsEvolved) return;

        IsEvolved = true;

        if (evolvedPrefab != null)
        {
            bellPrefab = evolvedPrefab;
        }

        foreach (BellAbility bell in activeBells)
        {
            if (bell != null)
            {
                Destroy(bell.gameObject);
            }
        }

        activeBells.Clear();

        SyncEvolvedBells();
    }
}
