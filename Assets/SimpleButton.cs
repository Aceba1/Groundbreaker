using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Sprite imageSwap;

    [Space]
    [SerializeField]
    [Tooltip("Debugging only. Desktop should have its own input system!")]
    private KeyCode keyboardTrigger = KeyCode.None;

    private Sprite imageOrig;
    private Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        imageOrig = image.sprite;
    }

    public event Action OnPressEvent;
    public event Action OnReleaseEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (imageSwap != null)
            image.sprite = imageSwap;
        OnPressEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (imageSwap != null)
            image.sprite = imageOrig;
        OnReleaseEvent?.Invoke();
    }
}
