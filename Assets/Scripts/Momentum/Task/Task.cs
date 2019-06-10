using System;
using System.Collections.Generic;

namespace Momentum
{
    /// <summary>
    /// Time object that updates every frame with exposed callbacks
    /// </summary>
    [Serializable]
    public class Task : IComparable<Task>
    {
        public State State => state;

        protected const float FixedDeltaTime = 1f / 60f;

        [UnityEngine.SerializeField]
        protected string name = string.Empty;

        [UnityEngine.SerializeField]
        protected State state;

        protected Action<State> onStart;
        protected Action<State> onUpdate;
        protected Action<State> onLoop;
        protected Action<State> onComplete;

        protected readonly HashSet<ITaskable> taskables;

        public Task(ITaskable taskable) : this()
        {
            AddTo(taskable);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Momentum.Task"/> class.
        /// </summary>
        public Task()
        {
            state = new State(this);
            taskables = new HashSet<ITaskable>();
        }

        /// <summary>
        /// Creates a new tasks and starts it right away
        /// </summary>
        public static Task Run()
        {
            return new Task()
                .Start();
        }

        /// <summary>
        /// Creates a new tasks; adds it to a taskable; starts it right away
        /// </summary>
        public static Task Run(ITaskable taskable)
        {
            return new Task(taskable)
                .Start();
        }

        /// <summary>
        /// Starts the task by adding it into the <see cref="T:Momentum.Juggler"/> and keeping track of taskable
        /// </summary>
        public Task Start(ITaskable taskable)
        {
            return Start()
                .AddTo(taskable);
        }

        /// <summary>
        /// Starts the task by adding it into the <see cref="T:Momentum.Juggler"/>
        /// </summary>
        public Task Start()
        {
            state.IsActive = true;

            Juggler.Instance.Add(this);

            return this;
        }

        /// <summary>
        /// Stops the tasks by removing it from the <see cref="T:Momentum.Juggler"/> and all of the taskables
        /// </summary>
        public virtual void Stop()
        {
            state.IsActive = false;

            Juggler.Instance.Remove(this);

            foreach (ITaskable taskable in taskables)
            {
                RemoveFrom(taskable);
            }
        }

        /// <summary>
        /// Gives a name to the task that is visible inside the inspector
        /// </summary>
        public Task Name(string name)
        {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Sets the execurion order of the task
        /// </summary>
        public Task Order(int order)
        {
            state.Order = order;

            if (state.IsActive)
            {
                Juggler.Instance.Remove(this);
                Juggler.Instance.Add(this);
            }

            return this;
        }

        /// <summary>
        /// Sets the duration of the task
        /// </summary>
        public Task Duration(float duration = 1f)
        {
            state.Duration = duration;
            return this;
        }

        /// <summary>
        /// Randomizes the duration of the task
        /// </summary>
        public Task Random(float duration = 0f)
        {
            state.Random = duration;
            return this;
        }

        /// <summary>
        /// Loop task x times (-1 for infinite)
        /// </summary>
        public Task Loop(int loops = -1)
        {
            state.Loops = loops;
            return this;
        }

        /// <summary>
        /// Adds task to taskable
        /// </summary>
        public Task AddTo(ITaskable taskable)
        {
            taskables.Add(taskable);
            taskable.AddTask(this);
            return this;
        }

        /// <summary>
        /// Removes task from taskable
        /// </summary>
        public Task RemoveFrom(ITaskable taskable)
        {
            if (taskables.Contains(taskable))
            {
                taskable.RemoveTask(this);
            }
            return this;
        }

        /// <summary>
        /// Callback for when the task is started
        /// </summary>
        public Task OnStart(Action<State> callback)
        {
            onStart += callback;
            return this;
        }

        /// <summary>
        /// Callback for when the task is updated
        /// </summary>
        public Task OnUpdate(Action<State> callback)
        {
            onUpdate += callback;
            return this;
        }

        /// <summary>
        /// Callback for when the task is looped
        /// </summary>
        public Task OnLoop(Action<State> callback)
        {
            onLoop += callback;
            return this;
        }

        /// <summary>
        /// Callback for when the task is completed
        /// </summary>
        public Task OnComplete(Action<State> callback)
        {
            onComplete += callback;
            return this;
        }

        /// <summary>
        /// Resets all values
        /// </summary>
        public void Reset()
        {
            state.IsActive = false;
            state.CurrentDuration = 0f;
            state.CurrentRandom = 0f;
            state.CurrentLoop = 0;
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        public void Update(float deltaTime)
        {
            TryStart();
            Advance(deltaTime);
            TryRepeat();
        }

        void TryStart()
        {
            if (state.CurrentDuration <= 0f && (state.Loops == 0 || state.CurrentLoop == 0))
            {
                state.CurrentRandom = UnityEngine.Random.Range(-state.Random, state.Random);

                onStart?.Invoke(state);
            }
        }

        protected void Advance(float deltaTime)
        {
            state.CurrentDuration += deltaTime;
            onUpdate?.Invoke(state);
        }

        protected void TryRepeat()
        {
            while (state.CurrentDuration >= state.Duration)
            {
                if (state.Loops == -1 || state.CurrentLoop < state.Loops)
                {
                    Repeat();
                }
                else if (state.CurrentLoop == state.Loops)
                {
                    Complete();
                    break;
                }
            }
        }

        protected void Repeat()
        {
            state.CurrentLoop++;

            state.CurrentDuration -= UnityEngine.Mathf.Clamp(state.Duration, FixedDeltaTime, state.Duration);

            state.CurrentRandom = UnityEngine.Random.Range(-state.Random, state.Random);

            if (state.Loops == -1 || state.CurrentLoop <= state.Loops)
            {
                onLoop?.Invoke(state);
            }
        }

        /// <summary>
        /// Successfully complete the task
        /// </summary>
        public void Complete()
        {
            onComplete?.Invoke(state);
            Stop();
        }

        /// <summary>
        /// Used for sorting inside <see cref="T:Momentum.Juggler"/>
        /// </summary>
        public int CompareTo(Task other)
        {
            return this.state.Order - other.state.Order;
        }

        /// <summary>
        /// Is the task currently running?
        /// </summary>
        public virtual bool IsActive()
        {
            return state.IsActive;
        }
    }
}