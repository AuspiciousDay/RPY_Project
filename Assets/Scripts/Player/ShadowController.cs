using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer characterSprite;

    [SerializeField] private float widthMultiplier = 0.8f;
    [SerializeField] private float heightMultiplier = 0.2f;
    [SerializeField] private float offsetY = 0f;

    private void LateUpdate()
    {
        if (characterSprite == null) return;

        Bounds bounds = characterSprite.bounds;

        transform.position = new Vector3(
            bounds.center.x,
            bounds.min.y + offsetY,
            transform.position.z
        );

        Vector3 parentScale = transform.parent.lossyScale;

        transform.localScale = new Vector3(
            bounds.size.x * widthMultiplier / Mathf.Abs(parentScale.x),
            bounds.size.x * heightMultiplier / Mathf.Abs(parentScale.y),
            1f
        );
    }
}