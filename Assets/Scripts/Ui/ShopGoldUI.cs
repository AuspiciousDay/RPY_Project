using UnityEngine;
using UnityEngine.UI;

public class ShopGoldUI : MonoBehaviour
{
    [SerializeField] private Text goldText;

    private void OnEnable()
    {
        if (GoldManager.Instance == null) return;

        GoldManager.Instance.OnGoldChanged += UpdateGold;

        UpdateGold(GoldManager.Instance.Gold);
    }

    private void OnDisable()
    {
        if (GoldManager.Instance == null) return;

        GoldManager.Instance.OnGoldChanged -= UpdateGold;
    }

    private void UpdateGold(int gold)
    {
        goldText.text = gold.ToString();
    }
}