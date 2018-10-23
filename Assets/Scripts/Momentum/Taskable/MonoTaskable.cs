using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    [DisallowMultipleComponent]
    public class MonoTaskable : MonoBehaviour, ITaskable
    {
        readonly HashSet<Task> tasks = new HashSet<Task>();

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
            HashSet<Task> tasksToStop = new HashSet<Task>(tasks);
            foreach (Task task in tasksToStop) task.Stop();
            tasks.Clear();
        }

        void OnDestroy()
        {
            StopAllTasks();
        }
    }
}