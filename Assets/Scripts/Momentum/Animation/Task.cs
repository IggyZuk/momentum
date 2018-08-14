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

        const float FixedDelta = 0.02f;

        public bool IsActive { get { return data.IsActive; } }

        public Task()
        {
            data = new TaskData(this);
        }

        public static Task Run()
        {
            Task task = new Task();
            task.Start();
            return task;
        }

        public void Start()
        {
            Core.Juggler.Add(this);
        }

        public void Stop()
        {
            Core.Juggler.Remove(this);
        }

        public Task Name(string name)
        {
            this.name = name;
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

        public Task Delay(float delay = 0f)
        {
            data.Delay = delay;
            return this;
        }

        public Task Loop(int loops = -1)
        {
            if (loops == -1)
            {
                data.Loops = int.MaxValue;
            }
            else
            {
                data.Loops = loops;
            }
            return this;
        }

        public Task Next(Task task)
        {
            data.Next = task;
            return task;
        }

        public Task Dispose(TaskDisposables container)
        {
            container.Add(this);
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

        public Task OnComplete(System.Action<TaskData> callback)
        {
            onComplete = callback;
            return this;
        }

        public Task OnRepeat(System.Action<TaskData> callback)
        {
            onRepeat = callback;
            return this;
        }

        public void Update(float deltaTime)
        {
            if (data.CurrentDelay < data.Delay)
            {
                data.CurrentDelay += deltaTime;
                return;
            }

            if (data.CurrentTime <= 0f && (data.Loops == 0 || data.CurrentLoop == 0))
            {
                data.CurrentRandom = UnityEngine.Random.Range(-data.Random, data.Random);

                if (onStart != null) onStart(data);
            }

            data.CurrentTime += deltaTime;

            if (onUpdate != null) onUpdate(data);

            while (data.CurrentTime >= data.Time)
            {
                if (data.CurrentLoop == data.Loops)
                {
                    data.IsActive = false;

                    if (onComplete != null) onComplete(data);

                    if (data.Next != null) data.Next.Start();

                    break;
                }
                else if (data.CurrentLoop < data.Loops)
                {
                    data.CurrentLoop++;

                    data.CurrentTime -= Mathf.Clamp(data.Time, FixedDelta, data.Time);

                    data.CurrentRandom = UnityEngine.Random.Range(-data.Random, data.Random);

                    if (data.CurrentLoop <= data.Loops)
                    {
                        if (onRepeat != null) onRepeat(data);
                    }
                }
            }
        }

        public void Reset()
        {
            data.IsActive = true;
            data.CurrentTime = 0f;
            data.CurrentRandom = 0f;
            data.CurrentDelay = 0f;
            data.CurrentLoop = 0;
        }
    }
}