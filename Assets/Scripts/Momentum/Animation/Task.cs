using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class Task
    {
        [SerializeField] bool _isActive = true;

        [SerializeField] string _name = string.Empty;

        [SerializeField] float _time = 0f;
        [SerializeField] float _currentTime = 0f;

        [SerializeField] float _random = 0f;
        [SerializeField] float _currentRandom = 0f;

        [SerializeField] float _delay = 0f;
        [SerializeField] float _currentDelay = 0f;

        [SerializeField] int _loops = 0;
        [SerializeField] int _currentLoops = 0;

        [SerializeField] Task _next = null;

        System.Action<Task> _onStart;
        System.Action<Task> _onUpdate;
        System.Action<Task> _onRepeat;
        System.Action<Task> _onComplete;

        public bool isActive { get { return _isActive; } }

        public float currentTime { get { return _currentTime; } }
        public float time { get { return _time + _currentRandom; } }
        public float progress { get { return _currentTime / Mathf.Clamp(Mathf.Epsilon, time, time); } }

        public float currentDelay { get { return _currentDelay; } }
        public float delay { get { return _delay; } }

        public float currentLoop { get { return _currentLoops; } }
        public float loops { get { return _loops; } }

        public static Task Add()
        {
            Task task = new Task();
            Core.Juggler.Add(task);
            return task;
        }

        public Task Name(string name)
        {
            _name = name;
            return this;
        }

        public Task Time(float time = 1f)
        {
            _time = time;
            return this;
        }

        public Task Random(float randomTime = 0f)
        {
            _random = randomTime;
            return this;
        }

        public Task Delay(float delay = 0f)
        {
            _delay = delay;
            return this;
        }

        public Task Loop(int loops = 0)
        {
            if (loops == -1)
            {
                _loops = int.MaxValue;
            }
            else
            {
                _loops = loops;
            }
            return this;
        }

        // TODO: have multiple children; go one by one though them
        public Task Next(Task task)
        {
            _next = task;
            return this;
        }

        public Task OnStart(System.Action<Task> callback)
        {
            _onStart = callback;
            return this;
        }

        public Task OnUpdate(System.Action<Task> callback)
        {
            _onUpdate = callback;
            return this;
        }

        public Task OnComplete(System.Action<Task> callback)
        {
            _onComplete = callback;
            return this;
        }

        public Task OnRepeat(System.Action<Task> callback)
        {
            _onRepeat = callback;
            return this;
        }

        public void Update(float deltaTime)
        {
            if (_currentDelay < _delay)
            {
                _currentDelay += deltaTime;
                return;
            }

            if (_currentTime <= 0f && (_loops == 0 || _currentLoops == 0))
            {
                _currentRandom = UnityEngine.Random.Range(-_random, _random);

                if (_onStart != null) _onStart(this);
            }

            _currentTime += deltaTime;

            if (_onUpdate != null) _onUpdate(this);

            if (_currentTime >= time)
            {
                if (_currentLoops == _loops)
                {
                    _isActive = false;

                    if (_onComplete != null) _onComplete(this);

                    if (_next != null) Core.Juggler.Add(_next);
                }
                else if (_currentLoops < _loops)
                {
                    _currentLoops++;

                    _currentTime -= _currentTime + deltaTime;

                    _currentRandom = UnityEngine.Random.Range(-_random, _random);

                    if (_currentLoops <= _loops)
                    {
                        if (_onRepeat != null) _onRepeat(this);
                    }
                }
            }
        }

        public void Reset()
        {
            _isActive = true;
            _currentTime = 0f;
            _currentRandom = 0f;
            _currentDelay = 0f;
            _currentLoops = 0;
        }
    }
}