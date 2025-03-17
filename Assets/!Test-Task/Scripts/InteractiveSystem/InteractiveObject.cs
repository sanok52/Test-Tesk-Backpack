using System.Collections;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{ }

public interface IOverable
{
    public void OnOverEnter(InteractiveSystem interactiveSystem);
    public void OnOver(InteractiveSystem interactiveSystem);
    public void OnOverEnd(InteractiveSystem interactiveSystem);
    public string GetTextInfo();
}

public interface IClickable
{
    public void OnClick(InteractiveSystem interactiveSystem);
    public void OnClickUp(InteractiveSystem interactiveSystem);
}

public interface IHoldable
{
    public void OnOverEnterHold(InteractiveSystem interactiveSystem);
    public void OnHold(InteractiveSystem interactiveSystem);
}