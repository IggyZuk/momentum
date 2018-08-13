using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class TaskData
    {
        Task task = null;

        [SerializeField] bool isActive = true;

        [SerializeField] float time = 0f;
        [SerializeField] float currentTime = 0f;

        [SerializeField] float random = 0f;
        [SerializeField] float currentRandom = 0f;

        [SerializeField] float delay = 0f;
        [SerializeField] float currentDelay = 0f;

        [SerializeField] int loops = 0;
        [SerializeField] int currentLoop = 0;

        [SerializeField] Task next = null;

        public TaskData(Task task)
        {
            this.task = task;
        }

        public Task Task
        {
            get { return task; }
        }

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

        public float CurrentDelay
        {
            get { return currentDelay; }
            set { currentDelay = value; }
        }

        public float Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public int Loops
        {
            get { return loops; }
            set { loops = value; }
        }

        public int CurrentLoop
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