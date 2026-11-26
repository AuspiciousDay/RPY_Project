using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TreasureUI : MonoBehaviour
{
    public GameObject treasurePanel;
    public GameObject titleText;
    public GameObject chestImage;
    public GameObject openButton;
    public GameObject rewardPanel;

    public Image[] lights;
    public Animator chestAnimator;

    [Header("Reward")]
    [SerializeField] private TreasureRewardManager rewardManager;
    [SerializeField] private Image[] rewardIcons;

    public bool IsTreasureOpen => treasurePanel.activeSelf;

    private bool isOpening;
    private bool isRewardReady;

    private void Start()
    {
        treasurePanel.SetActive(false);
        chestAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void ShowTreasure()
    {
        if (IsTreasureOpen) return;

        isOpening = false;
        isRewardReady = false;

        Time.timeScale = 0f;

        treasurePanel.SetActive(true);
        titleText.SetActive(true);
        chestImage.SetActive(true);
        openButton.SetActive(true);
        rewardPanel.SetActive(false);

        foreach (Image icon in rewardIcons)
        {
            icon.gameObject.SetActive(false);
        }

        chestAnimator.enabled = true;
        chestAnimator.Play("TreasureOpening", 0, 0f);
    }

    public void OpenTreasure()
    {
        if (!IsTreasureOpen || isOpening || isRewardReady) return;

        GameAudio.Play("chest_open");
        isOpening = true;

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

        int rewardCount = rewardManager.GetRewardCount();

        List<Sprite> rewards = rewardManager.GiveRewards(rewardCount);

        SetLights(rewards.Count);
        ShowRewards(rewards);

        rewardPanel.SetActive(true);

        isOpening = false;
        isRewardReady = true;
    }

    private void SetLights(int count)
    {
        lights[0].gameObject.SetActive(count >= 2);
        lights[1].gameObject.SetActive(count >= 1);
        lights[2].gameObject.SetActive(count == 3);
    }

    private void ShowRewards(List<Sprite> rewards)
    {
        foreach (Image icon in rewardIcons)
        {
            icon.gameObject.SetActive(false);
        }

        if (rewards.Count == 1)
        {
            rewardIcons[1].sprite = rewards[0];
            rewardIcons[1].gameObject.SetActive(true);
            return;
        }

        for (int i = 0; i < rewards.Count && i < rewardIcons.Length; i++)
        {
            rewardIcons[i].sprite = rewards[i];
            rewardIcons[i].gameObject.SetActive(true);
        }
    }

    public void CloseTreasure()
    {
        if (!isRewardReady) return;

        treasurePanel.SetActive(false);

        isOpening = false;
        isRewardReady = false;

        if (UiManager.Instance == null || !UiManager.Instance.IsRunOver)
            Time.timeScale = 1f;
    }
}
