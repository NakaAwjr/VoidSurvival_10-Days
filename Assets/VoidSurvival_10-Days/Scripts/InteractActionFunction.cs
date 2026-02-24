using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractActionFunction : InteractAction
{
    public UnityEvent func;
    public override void Action()
    {
        func.Invoke();
    }
}
