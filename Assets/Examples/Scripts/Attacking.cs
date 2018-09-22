using UnityEngine;
using UnityEngine.UI;

namespace Momentum
{
    public class Attacking : MonoTaskable
    {
        public float prepareTime = 2f;
        public float swingTime = 0.5f;
        public float cooldownTime = 1f;

        public Text text;

        void Start()
        {
            text.text = "Ready";

            Task attackTask = new Task(this);

            attackTask
                .Name("Preparing...")
                .Time(prepareTime)
                .OnUpdate(data => text.text = "Preparing... : " + data.Progress.ToString("0.0"))
                .Next(this)
                .Name("Swing!!!")
                .Time(swingTime)
                .OnUpdate(data => text.text = "Swing! : " + data.Progress.ToString("0.0"))
                .Next(this)
                .Name("Cooldown...")
                .Time(cooldownTime)
                .OnUpdate(data => text.text = "Cooldown... : " + data.Progress.ToString("0.0"))
                .OnComplete(_ => text.text = "Ready");

            Task.Run(this)
                .Name("State[input]")
                .Loop()
                .OnUpdate(_ =>
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!attackTask.IsActiveWithChildren()) attackTask.Start();
                    }
                });
        }

    }
}
