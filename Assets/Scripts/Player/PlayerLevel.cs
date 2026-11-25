using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public UiManager uiManager;
    public float exp;
    public int level;

    public PassiveUpgradeUI passiveUpgradeUI;

    private void Start()
    {
        exp = 0;
        level = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GainExp(10);
        }
    }
    public void GainExp(float amount)
    {
        exp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        uiManager.UpdateExp(exp, level * 50, level);
        if (exp >= level * 50)
        {
            exp -= level * 50;
            level++;
            uiManager.UpdateExp(exp, level * 50, level);
            GameManager.PauseGame(0f);
            passiveUpgradeUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Gem gem = collision.GetComponent<Gem>();

        if (gem != null)
        {
            gem.SetTarget(this);
        }
    }
}
