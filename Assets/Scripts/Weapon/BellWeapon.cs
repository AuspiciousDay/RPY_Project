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

    private float timer;

    private void Update()
    {
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
        for (int i = 0; i < attackCount; i++)
        {
            float angle = 360f / attackCount * i;

            BellAbility bell = Instantiate(bellPrefab, transform.position, Quaternion.identity);

            bell.Init(transform, angle, radius, rotateSpeed, damage, duration);
        }
    }
}