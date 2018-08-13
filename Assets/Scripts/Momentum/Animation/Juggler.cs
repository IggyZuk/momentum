using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class Juggler
    {
        [SerializeField] List<Task> tasks = new List<Task>();

        public void Add(Task task)
        {
            tasks.Add(task);
        }

        public void Remove(Task task)
        {
            tasks.Remove(task);
        }

        public void Update(float deltaTime)
        {
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                Task task = tasks[i];
                task.Update(deltaTime);

                if (!task.IsActive)
                {
                    task.Reset();
                    Remove(task);
                }
            }
        }

        public void Purge()
        {
            tasks.Clear();
        }
    }
}