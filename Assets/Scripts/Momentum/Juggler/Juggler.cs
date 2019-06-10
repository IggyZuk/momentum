using System;
using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    /// <summary>
    /// Updates all <see cref="T:Momentum.Task"/> objects
    /// </summary>
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

        /// <summary>
        /// Add a task into the list
        /// </summary>
        public void Add(Task task)
        {
            buffer.Add(task);
        }

        /// <summary>
        /// Remove a task for the list
        /// </summary>
        public void Remove(Task task)
        {
            buffer.Remove(task);
            tasks.Remove(task);
        }

        /// <summary>
        /// Go through all tasks and update them
        /// </summary>
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

                if (!task.State.IsActive)
                {
                    task.Reset();
                    Remove(task);
                    i--;
                }
            }
        }

        /// <summary>
        /// Purge all tasks
        /// </summary>
        public void Purge()
        {
            tasks.Clear();
            buffer.Clear();
        }

        /// <summary>
        /// Based on the order property of the task insert it into the list using binary search
        /// </summary>
        void AddSorted(Task task)
        {
            if (tasks.Count == 0)
            {
                tasks.Add(task);
                return;
            }
            if (tasks[tasks.Count - 1].State.Order < task.State.Order)
            {
                tasks.Add(task);
                return;
            }
            if (tasks[0].State.Order >= task.State.Order)
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