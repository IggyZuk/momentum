using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class Task
    {
        [SerializeField] string name = string.Empty;
        [SerializeField] TaskData data;

        System.Action<TaskData> onStart;
        System.Action<TaskData> onUpdate;
        System.Action<TaskData> onRepeat;
        System.Action<TaskData> onComplete;

        const float FixedDeltaTime = 0.02f;

        public TaskData Data { get { return data; } }

        public Task(ITaskable taskable) : this()
        {
            AddTo(taskable);
        }

        public Task()
        {
            data = new TaskData(this);
        }

        public static Task Run()
        {
            return new Task().Start();
        }

        public static Task Run(ITaskable taskable)
        {
            return new Task(taskable).Start();
        }

        public Task Start()
        {
            data.IsActive = true;

            Core.Juggler.Add(this);

            return this;
        }

        public void Stop()
        {
            data.IsActive = false;

            Core.Juggler.Remove(this);
        }

        public Task Name(string name)
        {
            this.name = name;
            return this;
        }

        public Task Order(int order)
        {
            data.Order = order;
            Core.Juggler.SortByOrder();
            return this;
        }

        public Task Time(float time = 1f)
        {
            data.Time = time;
            return this;
        }

        public Task Random(float randomTime = 0f)
        {
            data.Random = randomTime;
            return this;
        }

        public Task Loop(int loops = -1)
        {
            data.Loops = loops;
            return this;
        }

        public Task Next(ITaskable taskable)
        {
            return Next(new Task(taskable));
        }

        public Task Next()
        {
            return Next(new Task());
        }

        public Task Next(Task task)
        {
            data.Next = task;
            return task;
        }

        public Task AddTo(ITaskable taskable)
        {
            taskable.AddTask(this);
            return this;
        }

        public Task OnStart(System.Action<TaskData> callback)
        {
            onStart = callback;
            return this;
        }

        public Task OnUpdate(System.Action<TaskData> callback)
        {
            onUpdate = callback;
            return this;
        }

        public Task OnRepeat(System.Action<TaskData> callback)
        {
            onRepeat = callback;
            return this;
        }

        public Task OnComplete(System.Action<TaskData> callback)
        {
            onComplete = callback;
            return this;
        }

        public void Reset()
        {
            data.IsActive = false;
            data.CurrentTime = 0f;
            data.CurrentRandom = 0f;
            data.CurrentLoop = 0;
        }

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

                if (onStart != null) onStart(data);
            }
        }

        void Advance(float deltaTime)
        {
            data.CurrentTime += deltaTime;
            if (onUpdate != null) onUpdate(data);
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

            data.CurrentTime -= Mathf.Clamp(data.Time, FixedDeltaTime, data.Time);

            data.CurrentRandom = UnityEngine.Random.Range(-data.Random, data.Random);

            if (data.Loops == -1 || data.CurrentLoop <= data.Loops)
            {
                if (onRepeat != null) onRepeat(data);
            }
        }

        void Complete()
        {
            data.IsActive = false;

            if (onComplete != null) onComplete(data);
            if (data.Next != null) data.Next.Start();
        }
    }
}