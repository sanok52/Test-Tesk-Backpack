using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : InteractiveObject, IClickable, IOverable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public string Text;

    public UnityEvent OnClickEvent;
    public UnityEvent OnClickUpEvent;

    public void SetSprite (Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void OnClick(InteractiveSystem interactiveSystem)
    {
        if (interactiveSystem.IsCurrentOver(this))
            OnClickEvent?.Invoke();
    }

    public void OnClickUp(InteractiveSystem interactiveSystem)
    {
        if (interactiveSystem.IsCurrentOver(this))
            OnClickUpEvent?.Invoke();
    }

    public string GetTextInfo()
    {
        return Text;
    }

    public void OnOverEnter(InteractiveSystem interactiveSystem)
    { }

    public void OnOver(InteractiveSystem interactiveSystem)
    { }

    public void OnOverEnd(InteractiveSystem interactiveSystem)
    { }
}
