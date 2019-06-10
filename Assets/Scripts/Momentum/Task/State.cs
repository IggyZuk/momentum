using UnityEngine;

namespace Momentum
{
    /// <summary>
    /// State that <see cref="T:Momentum.Task"/> uses
    /// </summary>
    [System.Serializable]
    public class State
    {
        [SerializeField] bool isActive;

        [SerializeField] int order;

        [SerializeField] float totalDuration;
        [SerializeField] float currentDuration;

        [SerializeField] float totalRandom;
        [SerializeField] float currentRandom;

        [SerializeField] int totalLoops;
        [SerializeField] uint currentLoop;

        [System.NonSerialized] readonly Task task;

        public State(Task task)
        {
            this.task = task;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public float Duration
        {
            get { return totalDuration + currentRandom; }
            set { totalDuration = value; }
        }

        public float CurrentDuration
        {
            get { return currentDuration; }
            set { currentDuration = value; }
        }

        public float Random
        {
            get { return totalRandom; }
            set { totalRandom = value; }
        }

        public float CurrentRandom
        {
            get { return currentRandom; }
            set { currentRandom = value; }
        }

        public float Progress
        {
            get { return Mathf.Min(1f, currentDuration / Mathf.Max(Mathf.Epsilon, Duration)); }
        }

        public int Loops
        {
            get { return totalLoops; }
            set { totalLoops = value; }
        }

        public uint CurrentLoop
        {
            get { return currentLoop; }
            set { currentLoop = value; }
        }

        public Task Task { get { return task; } }
    }
}