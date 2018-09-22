using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class TaskData
    {
        [SerializeField] bool isActive;

        [SerializeField] int order;

        [SerializeField] float time;
        [SerializeField] float currentTime;

        [SerializeField] float random;
        [SerializeField] float currentRandom;

        [SerializeField] int loops;
        [SerializeField] uint currentLoop;

        [System.NonSerialized] Task task;
        [System.NonSerialized] Task next;

        public TaskData(Task task)
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

        public float CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        public float Random
        {
            get { return random; }
            set { random = value; }
        }

        public float CurrentRandom
        {
            get { return currentRandom; }
            set { currentRandom = value; }
        }

        public float Time
        {
            get { return time + currentRandom; }
            set { time = value; }
        }

        public float Progress
        {
            get { return currentTime / Mathf.Clamp(Mathf.Epsilon, Time, Time); }
        }

        public int Loops
        {
            get { return loops; }
            set { loops = value; }
        }

        public uint CurrentLoop
        {
            get { return currentLoop; }
            set { currentLoop = value; }
        }

        public Task Task { get { return task; } }

        public Task Next
        {
            get { return next; }
            set { next = value; }
        }
    }
}