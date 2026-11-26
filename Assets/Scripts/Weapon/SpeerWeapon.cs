using UnityEngine;

public class SpearWeapon : MonoBehaviour
{
    public float damage = 25f;
    public float attackInterval = 2f;
    public float speed = 10f;
    public float lifeTime = 3f;
    public float sizeMultiplier = 1f;
    public int attackCount = 1;

    [SerializeField] private SpearAbility spearPrefab;

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
        float randomAngle = Random.Range(0f, 360f);

        for (int i = 0; i < attackCount; i++)
        {
            float angle = randomAngle + 360f / attackCount * i;

            float radian = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radian),
                Mathf.Sin(radian)
            );

            SpearAbility spear = Instantiate(
                spearPrefab,
                transform.position,
                Quaternion.Euler(0f, 0f, angle - 90f)
            );

            spear.transform.localScale *= sizeMultiplier;

            spear.Init(direction, damage, speed, lifeTime);
        }
    }
}