using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class So : ScriptableObject
{
    public UnityAction OnEventRaised;
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
