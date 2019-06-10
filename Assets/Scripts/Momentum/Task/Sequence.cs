using System.Collections.Generic;

namespace Momentum
{
    /// <summary>
    /// A collection of tasks run one after another 
    /// </summary>
    [System.Serializable]
    public class Sequence : Task
    {
        readonly ITaskable taskable;
        readonly List<Task> tasks;

        Task Last => tasks[tasks.Count - 1];

        public Sequence(ITaskable taskable)
        {
            this.taskable = taskable;
            this.tasks = new List<Task>();

            Push(this);
        }

        void Push(Task task)
        {
            tasks.Add(task);
        }

        /// <summary>
        /// Appends a task
        /// </summary>
        public Sequence Append(Task task)
        {
            Last.OnComplete(_ => task.Start());

            Push(task);

            return this;
        }

        /// <summary>
        /// Appends a delay (seconds)
        /// </summary>
        public Sequence AppendDelay(float delay)
        {
            Task delayTask = new Task(taskable)
                .Name($"Sequence Delay: ({delay})")
                .Duration(delay);

            Last.OnComplete(_ => delayTask.Start());

            Push(delayTask);

            return this;
        }

        /// <summary>
        /// Appends a callback
        /// </summary>
        public Sequence AppendCallback(System.Action callback)
        {
            Last.OnComplete(_ => callback());

            return this;
        }

        /// <summary>
        /// Starts the sequence
        /// </summary>
        public new Sequence Start()
        {
            Last.OnComplete(_ => Stop());
            state.IsActive = true;
            onComplete?.Invoke(state);

            return this;
        }

        /// <summary>
        /// Is there a task active in the sequence?
        /// </summary>
        public override bool IsActive()
        {
            foreach (Task task in tasks)
            {
                if (task == this) continue;

                if (task != null)
                {
                    if (task.IsActive()) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Stops all tasks in the sequence
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            foreach (Task task in tasks)
            {
                if (task == this) continue;

                if (task != null)
                {
                    task.Stop();
                }
            }
        }
    }
}