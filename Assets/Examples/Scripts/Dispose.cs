using UnityEngine;

namespace Momentum.Tests
{
    public class Dispose : MonoBehaviour
    {
        public int life;

        void Start()
        {
            Task.Run(this.GetTaskable())
                .Name(name)
                .Duration(1f)
                .Loop(life)
                .OnLoop(data => Debug.LogFormat("{0} has {1} life left", name, life - data.CurrentLoop))
                .OnComplete(_ => Destroy(this.gameObject));
        }
    }
}