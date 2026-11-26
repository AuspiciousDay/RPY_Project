using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PowerUpShopView : MonoBehaviour
{
    [System.Serializable]
    private sealed class Entry
    {
        public Button button;
        public Image icon;
        public TMP_Text title;
        [TextArea] public string description;
    }

    [SerializeField] private Entry[] entries;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailTitle;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject startScreen;

    private static readonly Color SelectedColor = new(1f, 0.87f, 0.57f);

    private void Awake()
    {
        foreach (Entry entry in entries)
            entry.button.onClick.AddListener(() => Select(entry));

        backButton.onClick.AddListener(ReturnToStart);
    }

    private void OnEnable()
    {
        Select(entries[0]);
    }

    public void Open()
    {
        startScreen.SetActive(false);
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Select(Entry selected)
    {
        foreach (Entry entry in entries)
            entry.button.targetGraphic.color = entry == selected ? SelectedColor : Color.white;

        detailIcon.sprite = selected.icon.sprite;
        detailTitle.text = selected.title.text;
        detailDescription.text = selected.description;
    }

    public void ReturnToStart()
    {
        gameObject.SetActive(false);
        startScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
