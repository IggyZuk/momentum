using System.Collections.Generic;
using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class Juggler
    {
        [SerializeField] List<Task> _tasks = new List<Task>();

        public void Add(Task task)
        {
            _tasks.Add(task);
        }

        public void Remove(Task task)
        {            
            _tasks.Remove(task);
        }

        public void Update(float deltaTime)
        {
            for (int i = _tasks.Count - 1; i >= 0; i--)
            {
                Task task = _tasks[i];
                task.Update(deltaTime);

                if (!task.isActive)
                {
                    task.Reset();
                    Remove(task);
                }
            }
        }

        public void Purge()
        {
            _tasks.Clear();
        }
    }
}