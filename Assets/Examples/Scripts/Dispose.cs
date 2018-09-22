using UnityEngine;

namespace Momentum
{
    public class Dispose : MonoBehaviour
    {
        public int life;

        void Start()
        {
            Task.Run(this.GetTaskable())
                .Name(name)
                .Time(1f)
                .Loop(life)
                .OnRepeat(data => Debug.LogFormat("{0} has {1} life left", name, life - data.CurrentLoop))
                .OnComplete(_ => Destroy(this.gameObject));
        }
    }
}