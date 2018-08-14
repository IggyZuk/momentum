using System.Collections.Generic;

namespace Momentum
{
    public class TaskDisposables
    {
        List<Task> tasks = new List<Task>();

        public void Add(Task task)
        {
            tasks.Add(task);
        }

        public void Dispose()
        {
            foreach (var task in tasks)
            {
                task.Stop();
            }
            tasks.Clear();
        }
    }
}