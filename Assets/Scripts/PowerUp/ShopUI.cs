using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("강화 슬롯")]
    [SerializeField] private ShopSlot[] slots;

    [Header("선택한 강화 정보")]
    [SerializeField] private Image selectedIcon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text priceText;
    [SerializeField] private Button buyButton;

    private PowerUpData selectedData;

    private void Awake()
    {
        buyButton.onClick.AddListener(Buy);
    }

    private void OnEnable()
    {
        PowerUpManager.Instance.OnPowerUpChanged += OnPowerUpChanged;
        GoldManager.Instance.OnGoldChanged += OnGoldChanged;

        RefreshSlots();

        if (selectedData == null && slots.Length > 0)
        {
            SelectPowerUp(slots[0].Data);
        }
        else
        {
            RefreshDetails();
        }
    }

    private void OnDisable()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.OnPowerUpChanged -= OnPowerUpChanged;
        }

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.OnGoldChanged -= OnGoldChanged;
        }
    }

    public void SelectPowerUp(PowerUpData data)
    {
        selectedData = data;

        RefreshDetails();
    }

    private void RefreshDetails()
    {
        if (selectedData == null) return;

        PowerUpManager manager = PowerUpManager.Instance;

        int level = manager.GetLevel(selectedData.type);

        selectedIcon.sprite = selectedData.icon;

        nameText.text = $"{selectedData.powerUpName} ({level}/{selectedData.maxLevel})";

        descriptionText.text = selectedData.description;

        if (manager.IsMaxLevel(selectedData.type))
        {
            priceText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            priceText.text = $"{manager.GetPrice(selectedData.type):N0} G";
            buyButton.interactable = manager.CanPurchase(selectedData.type);
        }
    }

    private void Buy()
    {
        if (selectedData == null) return;

        PowerUpManager.Instance.Purchase(selectedData.type);
    }

    private void RefreshSlots()
    {
        foreach (ShopSlot slot in slots)
        {
            if (slot != null)
            {
                slot.Refresh();
            }
        }
    }

    private void OnPowerUpChanged(PowerUpType type)
    {
        RefreshSlots();
        RefreshDetails();
    }

    private void OnGoldChanged(int gold)
    {
        RefreshDetails();
    }
}