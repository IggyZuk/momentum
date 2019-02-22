using UnityEngine;

namespace Momentum
{
    public static class ComponentX
    {
        public static Task StartTask(this Component component)
        {
            return Task.Run(GetTaskable(component));
        }

        public static void StopAllTasks(this Component component)
        {
            GetTaskable(component).StopAllTasks();
        }

        public static ITaskable GetTaskable(this Component component)
        {
            return component.gameObject.GetComponent<MonoTaskable>() ??
                   component.gameObject.AddComponent<MonoTaskable>();
        }
    }
}