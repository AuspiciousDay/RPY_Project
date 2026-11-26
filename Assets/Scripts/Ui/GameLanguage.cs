using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameLanguage
{
    private const string Preference = "UI.Language";
    private static bool korean = PlayerPrefs.GetInt(Preference, 0) == 1;
    public event Action Changed;

    private readonly List<Action> labels = new List<Action>();
    private readonly Font koreanFont;
    private readonly TMP_FontAsset koreanTmpFont;

    public GameLanguage(Transform ui, GameObject shop)
    {
        koreanFont = Resources.Load<Font>("Localization/Galmuri11");
        if (koreanFont == null)
        {
            Debug.LogError("Missing UI font: Localization/Galmuri11");
            return;
        }

        koreanTmpFont = TMP_FontAsset.CreateFontAsset(koreanFont, 48, 5,
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024,
            AtlasPopulationMode.Dynamic, true);
        BindLabels(ui);
        if (shop != null) BindLabels(shop.transform);
    }

    public void AddButton(GameObject panel, Button style)
    {
        if (panel == null || koreanTmpFont == null) return;

        var control = new GameObject("Language", typeof(RectTransform), typeof(Image), typeof(Button));
        control.layer = panel.layer;
        var rect = (RectTransform)control.transform;
        rect.SetParent(panel.transform, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one;
        rect.anchoredPosition = new Vector2(-24f, -22f);
        rect.sizeDelta = new Vector2(188f, 52f);

        Image image = control.GetComponent<Image>();
        var button = control.GetComponent<Button>();
        button.targetGraphic = image;
        if (style != null && style.image != null)
        {
            image.sprite = style.image.sprite;
            image.type = style.image.type;
            image.color = style.image.color;
            button.colors = style.colors;
        }
        else
        {
            image.color = new Color(0.19f, 0.12f, 0.09f, 0.95f);
        }

        var textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = panel.layer;
        var label = textObject.GetComponent<TextMeshProUGUI>();
        label.rectTransform.SetParent(rect, false);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = new Vector2(6f, 3f);
        label.rectTransform.offsetMax = new Vector2(-6f, -3f);
        label.font = koreanTmpFont;
        label.fontSize = 21f;
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.fontSizeMax = 21f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;

        labels.Add(() => label.text = Choose("Language / 한국어", "언어 / English"));
        button.onClick.AddListener(() =>
        {
            korean = !korean;
            PlayerPrefs.SetInt(Preference, korean ? 1 : 0);
            PlayerPrefs.Save();
            GameAudio.Play("ui_click");
            Refresh();
            Changed?.Invoke();
        });
    }

    public void Refresh()
    {
        foreach (Action apply in labels) apply();
    }

    public void Dispose()
    {
        if (koreanTmpFont == null) return;
        // Runtime TMP assets own native atlas textures and a material.
        foreach (Texture2D atlas in koreanTmpFont.atlasTextures)
            if (atlas != null) UnityEngine.Object.Destroy(atlas);
        UnityEngine.Object.Destroy(koreanTmpFont.material);
        UnityEngine.Object.Destroy(koreanTmpFont);
    }

    private void BindLabels(Transform root)
    {
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            string english = text.text;
            string translated = TranslateKorean(english);
            TMP_FontAsset font = text.font;
            Material material = text.fontSharedMaterial;
            labels.Add(() =>
            {
                if (text == null) return;
                text.font = korean ? koreanTmpFont : font;
                if (!korean && material != null) text.fontSharedMaterial = material;
                if (english != translated && (text.text == english || text.text == translated))
                    text.text = Choose(english, translated);
            });
        }

        foreach (Text text in root.GetComponentsInChildren<Text>(true))
        {
            string english = text.text;
            string translated = TranslateKorean(english);
            Font font = text.font;
            labels.Add(() =>
            {
                if (text == null) return;
                text.font = korean ? koreanFont : font;
                if (english != translated && (text.text == english || text.text == translated))
                    text.text = Choose(english, translated);
            });
        }
    }

    public static string Choose(string english, string translated) => korean ? translated : english;

    public static string Translate(string english) => korean ? TranslateKorean(english) : english;

    private static string TranslateKorean(string english)
    {
        if (string.IsNullOrEmpty(english)) return english;
        return KoreanText.TryGetValue(english.Trim(), out string translated) ? translated : english;
    }

    private static readonly Dictionary<string, string> KoreanText = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Shop"] = "상점",
        ["START"] = "시작",
        ["EXIT"] = "종료",
        ["VAMPIRE \nSURVIVORS"] = "뱀파이어\n서바이버즈",
        ["Menu"] = "일시정지",
        ["Music"] = "음악",
        ["Sounds"] = "효과음",
        ["Continue"] = "계속하기",
        ["LEVEL UP!"] = "레벨 업!",
        ["NEW!"] = "신규!",
        ["Treasure Found!"] = "보물 발견!",
        ["OPEN"] = "열기",
        ["CONFIRM"] = "확인",
        ["RESTART"] = "다시 시작",
        ["QUIT"] = "메인 화면",
        ["GAME OVER"] = "게임 오버",
        ["Game Over!"] = "게임 오버!",
        ["Victory!"] = "승리!",
        ["PowerUp Selection"] = "영구 강화 선택",
        ["BACK"] = "뒤로",
        ["Buy"] = "구매",
        ["MAX"] = "최대",
        ["Max Level"] = "최대 레벨",
        ["Damage"] = "공격력",
        ["Might"] = "공격력",
        ["MaxHp"] = "최대 체력",
        ["Max Health"] = "최대 체력",
        ["Armor"] = "방어력",
        ["Speed"] = "이동 속도",
        ["Magnet"] = "자석",
        ["Area"] = "공격 범위",
        ["Count"] = "투사체 수",
        ["Amount"] = "투사체 수",
        ["Duration"] = "지속 시간",
        ["Cooldown"] = "재사용 대기시간",
        ["Velocity"] = "투사체 속도",
        ["Luck"] = "행운",
        ["Growth"] = "경험치",
        ["Cross"] = "십자가",
        ["Lightning"] = "번개",
        ["Whip"] = "채찍",
        ["Bell"] = "종",
        ["Spear"] = "창",
        ["HolyWater"] = "성수",
        ["Garlic"] = "마늘",
        ["EVCROSS"] = "진화 십자가",
        ["EVLIGHTNING"] = "진화 번개",
        ["EVWHIP"] = "진화 채찍",
        ["EVBELL"] = "진화 종",
        ["EVSPEAR"] = "진화 창",
        ["EVHOLYWATER"] = "진화 성수",
        ["EVGARLIC"] = "진화 마늘",
        ["Gain gold for the shop."] = "상점에서 쓸 골드를 얻습니다.",
        ["Restore health up to your maximum."] = "최대 체력까지 회복합니다.",
        ["Increases weapon damage by 30% of base damage per level."] = "레벨당 무기 피해량이 기본 피해량의 30%만큼 증가합니다.",
        ["Increases pickup range by 0.5"] = "아이템 획득 범위가 0.5 증가합니다.",
        ["Increases max HP by 20 and restores 20 HP"] = "최대 체력이 20 증가하고 체력을 20 회복합니다.",
        ["Increases movement speed by 0.5"] = "이동 속도가 0.5 증가합니다.",
        ["Increases weapon attack range and size by 10%"] = "무기 공격 범위와 크기가 10% 증가합니다.",
        ["Increases the number of projectiles by 1"] = "투사체가 1개 늘어납니다.",
        ["Increases weapon effect duration by 10%"] = "무기 효과 지속 시간이 10% 증가합니다.",
        ["Raises inflicted damage by 5% per level."] = "레벨당 가하는 피해가 5% 증가합니다.",
        ["Increases maximum health by 10% per level (max +30%)."] = "레벨당 최대 체력이 10% 증가합니다 (최대 +30%).",
        ["Reduces incoming damage by 1 per level (max 3)."] = "레벨당 받는 피해가 1 감소합니다 (최대 3).",
        ["Increases attack area by 5% per level (max +10%)."] = "레벨당 공격 범위가 5% 증가합니다 (최대 +10%).",
        ["Reduces weapon cooldown by 5% per level (max -10%)."] = "레벨당 무기 재사용 대기시간이 5% 감소합니다 (최대 -10%).",
        ["Increases weapon effect duration by 10% per level (max +20%)."] = "레벨당 무기 효과 지속 시간이 10% 증가합니다 (최대 +20%).",
        ["Increases projectile speed by 10% per level (max +20%)."] = "레벨당 투사체 속도가 10% 증가합니다 (최대 +20%).",
        ["Increases movement speed by 5% per level (max +10%)."] = "레벨당 이동 속도가 5% 증가합니다 (최대 +10%).",
        ["Increases item pickup range by 25% per level (max +50%)."] = "레벨당 아이템 획득 범위가 25% 증가합니다 (최대 +50%).",
        ["Increases luck by 10% per level (max +30%)."] = "레벨당 행운이 10% 증가합니다 (최대 +30%).",
        ["Increases experience gained by 5% per level (max +20%)."] = "레벨당 획득 경험치가 5% 증가합니다 (최대 +20%).",
        ["Summons holy books that orbit the player."] = "플레이어 주위를 도는 성서를 소환합니다.",
        ["Adds 1 additional holy book."] = "성서가 1권 늘어납니다.",
        ["Increases damage by 5."] = "피해량이 5 증가합니다.",
        ["Increases damage by 10."] = "피해량이 10 증가합니다.",
        ["Increases damage by 15."] = "피해량이 15 증가합니다.",
        ["Increases damage by 20."] = "피해량이 20 증가합니다.",
        ["Increases duration by 1s."] = "지속 시간이 1초 증가합니다.",
        ["Adds 1 book and increases rotation speed by 30%."] = "성서가 1권 늘고 회전 속도가 30% 증가합니다.",
        ["Unlocks Garlic."] = "마늘을 획득합니다.",
        ["Increases attack range."] = "공격 범위가 넓어집니다.",
        ["Fires bullets at the nearest enemy."] = "가장 가까운 적에게 투사체를 발사합니다.",
        ["Fires 1 additional projectile."] = "투사체를 1개 더 발사합니다.",
        ["Increases penetration by 1."] = "관통 횟수가 1 증가합니다.",
        ["Reduces attack interval by 0.2s."] = "공격 간격이 0.2초 줄어듭니다.",
        ["Reduces attack interval by 0.3s."] = "공격 간격이 0.3초 줄어듭니다.",
        ["Reduces attack interval by 1s."] = "공격 간격이 1초 줄어듭니다.",
        ["Throws holy water that creates damaging zones."] = "성수를 던져 적에게 피해를 주는 장판을 만듭니다.",
        ["Throws 1 additional holy water bottle."] = "성수를 1병 더 던집니다.",
        ["Increases area by 25% and duration by 1s."] = "범위가 25%, 지속 시간이 1초 증가합니다.",
        ["Throws 1 additional bottle and reduces cooldown by 0.5s."] = "성수를 1병 더 던지고 재사용 대기시간이 0.5초 줄어듭니다.",
        ["Strikes random enemies with lightning."] = "무작위 적에게 번개를 내리칩니다.",
        ["Strikes 1 additional enemy."] = "적 1명을 더 타격합니다.",
        ["Strikes 2 additional enemies."] = "적 2명을 더 타격합니다.",
        ["Fires piercing spears in random directions."] = "무작위 방향으로 관통하는 창을 발사합니다.",
        ["Fires 1 additional spear."] = "창을 1개 더 발사합니다.",
        ["Increases projectile speed by 30% and size by 20%."] = "투사체 속도가 30%, 크기가 20% 증가합니다.",
        ["Fires 1 additional spear and reduces cooldown by 0.5s."] = "창을 1개 더 발사하고 재사용 대기시간이 0.5초 줄어듭니다.",
        ["Slashes enemies in front of the player."] = "플레이어 앞의 적을 채찍으로 공격합니다.",
        ["Attacks in both directions."] = "양쪽 방향을 공격합니다.",
        ["Increases attack dimensions by 25%."] = "공격 범위의 가로와 세로가 25% 증가합니다."
    };
}
