using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class TaskData
    {
        [SerializeField] bool isActive;

        [SerializeField] float time;
        [SerializeField] float currentTime;

        [SerializeField] float random;
        [SerializeField] float currentRandom;

        [SerializeField] int loops;
        [SerializeField] uint currentLoop;

        [SerializeField] Task next;

        public TaskData(Task task)
        {
            this.Task = task;
        }

        public Task Task { get; private set; }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
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

        public Task Next
        {
            get { return next; }
            set { next = value; }
        }
    }
}