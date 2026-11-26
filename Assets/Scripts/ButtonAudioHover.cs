using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudioHover : MonoBehaviour, IPointerEnterHandler
{
    public Button Button { private get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Button != null && Button.IsInteractable())
            GameAudio.Play("ui_hover", 0.35f, 0.06f);
    }
}
