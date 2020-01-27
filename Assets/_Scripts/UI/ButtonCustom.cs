using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public abstract class ButtonCustomBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    public event Action<PointerEventData> onPointerDown;
    public event Action<PointerEventData> onPointerUp;
    public event Action<PointerEventData> onPointerExit;
    public event Action onClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}

public class ButtonCustom : ButtonCustomBase
{
    bool pointerEnter;
    bool holding;

    void Start()
    {
        onPointerDown += ((PointerEventData data) =>
         {
             holding = true;
             transform.DOKill();
             transform.DOScale(.95f, .1f);
         });

        onPointerUp += ((PointerEventData data) =>
        {
            holding = false;
            transform.DOKill();
            transform.DOScale(1f, .3f).SetEase(Ease.OutElastic);
        });

        onPointerExit += ((PointerEventData data) =>
        {
            if (holding)
            {
                OnPointerUp(data);
            }
        });
    }

}
