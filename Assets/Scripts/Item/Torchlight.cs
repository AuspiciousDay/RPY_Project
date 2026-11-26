using UnityEngine;
using System.Collections;

public class Torchlight : MonoBehaviour
{
    public int Health = 20;

    public GameObject CoinPrefab;
    public GameObject HealthItem50Prefab;
    public GameObject HealthItem100Prefab;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;

        Health -= damage;

        if (Health <= 0)
        {
            StartCoroutine(ColorTorchlight());
        }
    }

    private IEnumerator ColorTorchlight()
    {
        spriteRenderer.material.color = Color.white;

        yield return new WaitForSeconds(0.1f);

        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        int random = Random.Range(0, 100);

        if (random < 40)
        {
            Instantiate(CoinPrefab, transform.position, Quaternion.identity);
        }
        else if (random < 80)
        {
            Instantiate(HealthItem50Prefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(HealthItem100Prefab, transform.position, Quaternion.identity);
        }
    }
}