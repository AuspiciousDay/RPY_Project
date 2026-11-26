using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorting : MonoBehaviour
{
    [SerializeField] private float offsetY = 0f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-(transform.position.y + offsetY) * 100f);
    }
}