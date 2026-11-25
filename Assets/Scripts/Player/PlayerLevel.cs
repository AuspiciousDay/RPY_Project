using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public UiManager uiManager;
    public float exp;
    public int level;

    public WeaponUpgrade weaponUpgrade;

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
            weaponUpgrade.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Gem10"))
        {
            GainExp(10);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Gem30"))
        {
            GainExp(30);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Gem50"))
        {
            GainExp(50);
            Destroy(collision.gameObject);
        }
    }
}
