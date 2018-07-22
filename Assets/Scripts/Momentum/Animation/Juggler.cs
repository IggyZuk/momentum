using System.Collections.Generic;

namespace Momentum
{
    [System.Serializable]
    public class Juggler
    {
        public List<Tween> _tweens = new List<Tween>();

        public void Add(Tween animatable)
        {
            _tweens.Add(animatable);
        }

        public void Remove(Tween animatable)
        {
            _tweens.Remove(animatable);
        }

        public void Update(float deltaTime)
        {
            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                Tween tween = _tweens[i];
                tween.Update(deltaTime);

                if (!tween.IsActive)
                {
                    Remove(tween);
                }
            }
        }

        public void Purge()
        {
            _tweens.Clear();
        }
    }
}