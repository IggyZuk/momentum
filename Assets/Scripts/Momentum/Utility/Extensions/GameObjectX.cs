using UnityEngine;

namespace Momentum
{
    public static class GameObjectX
    {
        public static Task StartTask(this GameObject gameObject)
        {
            return Task.Run(GetTaskable(gameObject));
        }

        public static void StopAllTasks(this GameObject gameObject)
        {
            GetTaskable(gameObject).StopAllTasks();
        }

        public static ITaskable GetTaskable(this GameObject gameObject)
        {
            return gameObject.GetComponent<MonoTaskable>() ??
                   gameObject.AddComponent<MonoTaskable>();
        }
    }
}