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
            tasks.Add(task);
            SortByOrder();
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

        public void SortByOrder()
        {
            tasks.Sort((a, b) => a.Data.Order.CompareTo(b.Data.Order));
        }
    }
}