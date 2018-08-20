using System;
using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    [Serializable]
    public class Juggler
    {
        [SerializeField] List<Task> tasks = new List<Task>();

        public void Add(Task task)
        {
            AddSorted(task);
        }

        public void Remove(Task task)
        {
            tasks.Remove(task);
        }

        public void Update(float deltaTime)
        {
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

        public void Purge()
        {
            tasks.Clear();
        }

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