﻿using System.Collections.Generic;

namespace Momentum
{
    public class Taskable : ITaskable
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
            foreach (Task task in tasks)
            {
                if (task != null) task.Stop();
            }
            tasks.Clear();
        }
    }
}