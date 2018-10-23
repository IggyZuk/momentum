using UnityEngine;
using Momentum;

public static class ComponentX
{
    public static Task StartTask(this Component component)
    {
        return Task.Run(GetTaskable(component));
    }

    public static ITaskable GetTaskable(this Component component)
    {
        return component.gameObject.GetComponent<MonoTaskable>() ??
               component.gameObject.AddComponent<MonoTaskable>();
    }
}
