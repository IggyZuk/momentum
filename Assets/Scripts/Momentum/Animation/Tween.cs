using UnityEngine;

namespace Momentum
{
    [System.Serializable]
    public class Tween
    {
        [SerializeField] bool _isActive = true;

        [SerializeField] float _time = 0f;
        [SerializeField] float _currentTime = 0f;

        [SerializeField] float _random = 0f;
        [SerializeField] float _currentRandom = 0f;

        [SerializeField] float _delay = 0f;
        [SerializeField] float _currentDelay = 0f;

        [SerializeField] int _loops = 0;
        [SerializeField] int _currentLoops = 0;

        [SerializeField] Tween _next = null;

        System.Action _onStart;
        System.Action<float> _onUpdate;
        System.Action<int> _onRepeat;
        System.Action _onComplete;

        public bool IsActive { get { return _isActive; } }

        public float CurrentTime { get { return _currentTime; } }
        public float TotalTime { get { return _time; } }

        public float CurrentDelay { get { return _currentDelay; } }
        public float TotalDelay { get { return _delay; } }

        public float CurrentLoop { get { return _currentLoops; } }
        public float TotalLoops { get { return _loops; } }

        public Tween Time(float time = 1f)
        {
            _time = time;
            return this;
        }

        public Tween Random(float randomTime = 0f)
        {
            _random = randomTime;
            return this;
        }

        public Tween Delay(float delay = 0f)
        {
            _delay = delay;
            return this;
        }

        public Tween Loop(int loops = 0)
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

        public Tween Next(Tween tween)
        {
            _next = tween;
            return this;
        }

        public Tween OnStart(System.Action callback)
        {
            _onStart = callback;
            return this;
        }

        public Tween OnUpdate(System.Action<float> callback)
        {
            _onUpdate = callback;
            return this;
        }

        public Tween OnComplete(System.Action callback)
        {
            _onComplete = callback;
            return this;
        }

        public Tween OnRepeat(System.Action<int> callback)
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

                if (_onStart != null) _onStart();
            }

            _currentTime += deltaTime;

            if (_onUpdate != null) _onUpdate(_currentTime);

            if (_currentTime >= _time + _currentRandom)
            {
                if (_currentLoops == _loops)
                {
                    _isActive = false;

                    if (_onComplete != null) _onComplete();

                    if (_next != null) Core.Juggler.Add(_next);
                }
                else if (_currentLoops < _loops)
                {
                    _currentLoops++;

                    _currentTime -= _currentTime + deltaTime;

                    _currentRandom = UnityEngine.Random.Range(-_random, _random);

                    if (_currentLoops <= _loops)
                    {
                        if (_onRepeat != null) _onRepeat(_currentLoops);
                    }
                }
            }
        }

        public void Reset()
        {
            _currentTime = 0f;
            _currentDelay = 0f;
            _currentLoops = 0;
            _isActive = true;
        }
    }
}