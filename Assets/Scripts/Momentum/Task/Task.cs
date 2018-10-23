using System;
using System.Collections.Generic;

namespace Momentum
{
    [Serializable]
    public class Task : IComparable<Task>
    {
        const float FixedDeltaTime = 0.02f;

        [UnityEngine.SerializeField] string name = string.Empty;
        [UnityEngine.SerializeField] TaskData data;

        Action<TaskData> onStart;
        Action<TaskData> onUpdate;
        Action<TaskData> onRepeat;
        Action<TaskData> onComplete;

        HashSet<ITaskable> taskables;

        public TaskData Data { get { return data; } }

        public Task(ITaskable taskable) : this()
        {
            AddTo(taskable);
        }

        public Task()
        {
            data = new TaskData(this);
            taskables = new HashSet<ITaskable>();
        }

        // Creates a new tasks and starts it right away
        public static Task Run()
        {
            return new Task().Start();
        }

        // Creates a new tasks; adds it to a taskable; starts it right away
        public static Task Run(ITaskable taskable)
        {
            return new Task(taskable).Start();
        }

        // Starts the task by adding it into the juggler
        public Task Start()
        {
            data.IsActive = true;

            Juggler.Instance.Add(this);

            return this;
        }

        // Stops the tasks by removing it from the juggler and all of the taskables
        public void Stop()
        {
            data.IsActive = false;

            Juggler.Instance.Remove(this);

            foreach (ITaskable taskable in taskables)
            {
                RemoveFrom(taskable);
            }
        }

        // Stops the task and all of its children
        public void StopAll()
        {
            if (IsActive()) Stop();

            HashSet<Task> tasks = new HashSet<Task> { this };

            Task next = data.Next;

            while (next != null)
            {
                if (next.IsActive()) next.Stop();

                if (tasks.Contains(next)) break;

                tasks.Add(next);
                next = next.data.Next;
            }
        }

        // Gives a name to the task that is visible inside the inspector
        public Task Name(string name)
        {
            this.name = name;
            return this;
        }

        // Sets the execution order for this task
        public Task Order(int order)
        {
            data.Order = order;

            if (data.IsActive)
            {
                Juggler.Instance.Remove(this);
                Juggler.Instance.Add(this);
            }

            return this;
        }

        // Sets the duration this task will run for
        public Task Time(float time = 1f)
        {
            data.Time = time;
            return this;
        }

        // Sets the randomized value that affects the duration of the task: Time + Rand(-randomTime, +randomTime)
        public Task Random(float randomTime = 0f)
        {
            data.Random = randomTime;
            return this;
        }

        // Sets the amount of loops this task will perform; -1 is infinite
        public Task Loop(int loops = -1)
        {
            data.Loops = loops;
            return this;
        }

        // Creates a new task; sets it as next, and returns it
        public Task Next()
        {
            return Next(new Task());
        }

        // Sets the next task that will run after this one completes; adds it to a taskable
        public Task Next(ITaskable taskable)
        {
            return Next(new Task(taskable));
        }

        // Sets the next task that will run after this one completes
        public Task Next(Task task)
        {
            data.Next = task;
            return task;
        }

        // Adds task to a specific taskable
        public Task AddTo(ITaskable taskable)
        {
            taskables.Add(taskable);
            taskable.AddTask(this);
            return this;
        }

        // Removes task from a specific taskable
        public Task RemoveFrom(ITaskable taskable)
        {
            if (taskables.Contains(taskable))
            {
                taskable.RemoveTask(this);
            }
            return this;
        }

        // Sets a callback that will execute on start
        public Task OnStart(Action<TaskData> callback)
        {
            onStart = callback;
            return this;
        }

        // Sets a callback that will execute every frame
        public Task OnUpdate(Action<TaskData> callback)
        {
            onUpdate = callback;
            return this;
        }

        // Sets a callback that will execute on repeat
        public Task OnRepeat(Action<TaskData> callback)
        {
            onRepeat = callback;
            return this;
        }

        // Sets a callback that will execute when the task is completed
        public Task OnComplete(Action<TaskData> callback)
        {
            onComplete = callback;
            return this;
        }

        // Resets all values
        public void Reset()
        {
            data.IsActive = false;
            data.CurrentTime = 0f;
            data.CurrentRandom = 0f;
            data.CurrentLoop = 0;
        }

        // Updates the task
        public void Update(float deltaTime)
        {
            TryStart();
            Advance(deltaTime);
            TryRepeat();
        }

        void TryStart()
        {
            if (data.CurrentTime <= 0f && (data.Loops == 0 || data.CurrentLoop == 0))
            {
                data.CurrentRandom = UnityEngine.Random.Range(-data.Random, data.Random);

                onStart?.Invoke(data);
            }
        }

        void Advance(float deltaTime)
        {
            data.CurrentTime += deltaTime;
            onUpdate?.Invoke(data);
        }

        void TryRepeat()
        {
            while (data.CurrentTime >= data.Time)
            {
                if (data.Loops == -1 || data.CurrentLoop < data.Loops)
                {
                    Repeat();
                }
                else if (data.CurrentLoop == data.Loops)
                {
                    Complete();
                    break;
                }
            }
        }

        void Repeat()
        {
            data.CurrentLoop++;

            data.CurrentTime -= UnityEngine.Mathf.Clamp(data.Time, FixedDeltaTime, data.Time);

            data.CurrentRandom = UnityEngine.Random.Range(-data.Random, data.Random);

            if (data.Loops == -1 || data.CurrentLoop <= data.Loops)
            {
                onRepeat?.Invoke(data);
            }
        }

        void Complete()
        {
            onComplete?.Invoke(data);

            Stop();

            if (data.Next != null) data.Next.Start();
        }

        // Compare is used within the Juggler for sorting
        public int CompareTo(Task other)
        {
            return this.data.Order - other.data.Order;
        }

        // Says whether task is active
        public bool IsActive()
        {
            return data.IsActive;
        }

        // Says whether task is active or any of its children
        public bool IsActiveWithChildren()
        {
            if (IsActive()) return true;

            HashSet<Task> tasks = new HashSet<Task> { this };

            Task next = data.Next;

            while (next != null)
            {
                if (next.IsActive()) return true;

                if (tasks.Contains(next)) return false;

                tasks.Add(next);
                next = next.data.Next;
            }

            return false;
        }
    }
}