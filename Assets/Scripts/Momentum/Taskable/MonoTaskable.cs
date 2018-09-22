using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    public class MonoTaskable : MonoBehaviour, ITaskable
    {
        readonly List<Task> tasks = new List<Task>();

        public void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public void RemoveTask(Task task)
        {
            tasks.Remove(task);
        }

        public void StopAllTasks()
        {
            foreach (Task task in tasks)
            {
                if (task != null) task.Stop();
            }
            tasks.Clear();
        }

        void OnDestroy()
        {
            StopAllTasks();
        }
    }
}