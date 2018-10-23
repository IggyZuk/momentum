using System;
using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    [Serializable]
    public class Juggler
    {
        public static Juggler Instance;

        List<Task> buffer = new List<Task>();
        [SerializeField] List<Task> tasks = new List<Task>();

        public Juggler()
        {
            Instance = this;
        }

        // Add a task into the list
        public void Add(Task task)
        {
            buffer.Add(task);
        }

        // Remove a task for the list
        public void Remove(Task task)
        {
            buffer.Remove(task);
            tasks.Remove(task);
        }

        // Go through all tasks and update them
        public void Update(float deltaTime)
        {
            if (buffer.Count > 0)
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    AddSorted(buffer[i]);
                }

                buffer.Clear();
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                Task task = tasks[i];
                task.Update(deltaTime);

                if (!task.Data.IsActive)
                {
                    task.Reset();
                    Remove(task);
                    i--;
                }
            }
        }

        // Purge all tasks
        public void Purge()
        {
            tasks.Clear();
            buffer.Clear();
        }

        // Based on the order property of the task insert it into the list using binary search
        void AddSorted(Task task)
        {
            if (tasks.Count == 0)
            {
                tasks.Add(task);
                return;
            }
            if (tasks[tasks.Count - 1].Data.Order < task.Data.Order)
            {
                tasks.Add(task);
                return;
            }
            if (tasks[0].Data.Order >= task.Data.Order)
            {
                tasks.Insert(0, task);
                return;
            }

            int index = tasks.BinarySearch(task);
            if (index < 0) index = ~index;
            tasks.Insert(index, task);
        }
    }
}