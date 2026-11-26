using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TreasureUI : MonoBehaviour
{
    public GameObject treasurePanel;
    public GameObject titleText;
    public GameObject chestImage;
    public GameObject openButton;
    public GameObject rewardPanel;

    public Image[] lights;
    public Animator chestAnimator;

    private void Start()
    {
        treasurePanel.SetActive(false);
    }

    public void ShowTreasure()
    {
        treasurePanel.SetActive(true);

        titleText.SetActive(true);
        chestImage.SetActive(true);
        openButton.SetActive(true);
        rewardPanel.SetActive(false);

        Time.timeScale = 0f;

        chestAnimator.enabled = true;
        chestAnimator.Play("TreasureOpening", 0, 0f);
    }

    public void OpenTreasure()
    {
        openButton.SetActive(false);
        titleText.SetActive(false);

        StartCoroutine(OpenAnimation());
    }

    private IEnumerator OpenAnimation()
    {
        chestAnimator.enabled = false;

        chestImage.transform.localRotation = Quaternion.identity;

        chestAnimator.enabled = true;

        chestAnimator.Play("TreasureLidOpen", 0, 0f);

        yield return new WaitForSecondsRealtime(1f);

        chestAnimator.enabled = false;

        chestImage.SetActive(false);
        RandomLight();
        rewardPanel.SetActive(true);
    }

    private void RandomLight()
    {
        int random = Random.Range(0, 2);

        lights[0].gameObject.SetActive(random == 1);
        lights[1].gameObject.SetActive(true);
        lights[2].gameObject.SetActive(random == 1);
    }

    public void CloseTreasure()
    {
        treasurePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}