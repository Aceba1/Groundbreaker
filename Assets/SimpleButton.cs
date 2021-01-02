using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class SimpleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField]
    private Sprite imageSwap;

    private Sprite imageOrig;
    private Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        imageOrig = image.sprite;
    }

    public event Action OnPressEvent;
    public event Action OnReleaseEvent;
    public event Action OnClickEvent;
    public event Action<bool> OnChangeEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (imageSwap != null)
            image.sprite = imageSwap;
        OnPressEvent?.Invoke();
        OnChangeEvent?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (imageSwap != null)
            image.sprite = imageOrig;
        OnReleaseEvent?.Invoke();
        OnChangeEvent?.Invoke(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent?.Invoke();
    }
}
