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

    public bool IsEvolved { get; private set; }

    private readonly List<BellAbility> activeBells = new List<BellAbility>();

    private void Update()
    {
        if (Time.timeScale == 0f || IsEvolved) return;

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

            bell.Init(transform, angle, radius, rotateSpeed, damage, duration, IsEvolved);

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

        Attack();
    }
}