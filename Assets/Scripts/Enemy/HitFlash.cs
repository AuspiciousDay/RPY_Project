using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private float flashDuration = 0.1f;
    private Color flashColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private Material flashMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            originalColor = spriteRenderer.color;
            flashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        }
    }

    public void Flash()
    {
        if (spriteRenderer == null) return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.material = flashMaterial;
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.material = originalMaterial;
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }
}