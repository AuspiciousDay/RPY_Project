using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardEffect : MonoBehaviour
{
    public Image movingIcon;
    public Sprite[] itemSprites;
    public Image[] lights;

    public float moveSpeed = 200f;
    public float spawnInterval = 0.2f;
    public float moveDistance = 350f;

    private void OnEnable()
    {
        movingIcon.gameObject.SetActive(false);

        StartCoroutine(SpawnIcons());
    }

    private IEnumerator SpawnIcons()
    {
        while (true)
        {
            SpawnIcon();

            yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }

    private void SpawnIcon()
    {
        if (itemSprites.Length == 0) return;

        foreach (Image light in lights)
        {
            if (light == null) continue;

            if (!light.gameObject.activeInHierarchy) continue;

            Image icon = Instantiate(movingIcon, movingIcon.transform.parent);

            int index = Random.Range(0, itemSprites.Length);

            icon.sprite = itemSprites[index];
            icon.rectTransform.position = light.rectTransform.position;
            icon.gameObject.SetActive(true);

            Vector2 direction = light.rectTransform.localRotation * Vector3.up;

            StartCoroutine(MoveIcon(icon, direction));
        }
    }

    private IEnumerator MoveIcon(Image icon, Vector2 direction)
    {
        RectTransform rect = icon.rectTransform;

        Vector2 startPosition = rect.anchoredPosition;

        while (Vector2.Distance(startPosition, rect.anchoredPosition) < moveDistance)
        {
            rect.anchoredPosition += direction * moveSpeed * Time.unscaledDeltaTime;

            yield return null;
        }

        Destroy(icon.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        if (movingIcon == null) return;

        Transform root = movingIcon.transform.parent;

        foreach (Transform child in root)
        {
            if (child != movingIcon.transform &&
                child.name == movingIcon.name + "(Clone)")
            {
                Destroy(child.gameObject);
            }
        }
    }
}