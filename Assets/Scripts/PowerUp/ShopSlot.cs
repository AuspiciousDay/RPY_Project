using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopSlot : MonoBehaviour
{
    [SerializeField] private PowerUpData data;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Image icon;

    [Header("강화 레벨 표시")]
    [SerializeField] private Image[] levelIndicators;

    [SerializeField] private Color lockedColor = new Color32(65, 65, 80, 255);
    [SerializeField] private Color unlockedColor = new Color32(230, 185, 75, 255);

    private Button button;

    public PowerUpData Data => data;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (data == null || PowerUpManager.Instance == null) return;

        int level = PowerUpManager.Instance.GetLevel(data.type);

        if (icon != null)
        {
            icon.sprite = data.icon;
        }

        for (int i = 0; i < levelIndicators.Length; i++)
        {
            if (levelIndicators[i] == null) continue;

            levelIndicators[i].color = i < level ? unlockedColor : lockedColor;
        }
    }

    private void OnClick()
    {
        if (shopUI == null || data == null) return;

        GameAudio.Play("ui_click");
        shopUI.SelectPowerUp(data);
    }
}