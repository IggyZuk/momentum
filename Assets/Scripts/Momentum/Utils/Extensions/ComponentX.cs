using UnityEngine;
using Momentum;

public static class ComponentX
{
    public static ITaskable GetTaskable(this Component component)
    {
        return component.gameObject.GetComponent<MonoTaskable>() ??
               component.gameObject.AddComponent<MonoTaskable>();
    }
}
