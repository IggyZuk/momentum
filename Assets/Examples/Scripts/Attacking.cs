using UnityEngine;
using UnityEngine.UI;

namespace Momentum.Tests
{
    public class Attacking : MonoTaskable
    {
        public float prepareTime = 2f;
        public float swingTime = 0.5f;
        public float cooldownTime = 1f;

        public Text text;

        public Transform target;

        public Transform rest;
        public Transform prepare;
        public Transform swing;

        void Start()
        {
            text.text = "Ready";

            Lerp(target, rest, 1f);

            Task attackTask = new Task(this);

            attackTask
                .Name("Preparing...")
                .Time(prepareTime)
                .OnUpdate(data =>
                {
                    text.text = "Preparing... : " + data.Progress.ToString("0.0");
                    Lerp(rest, prepare, Ease.InOutSine(data.Progress));
                })
                .Next(this)
                .Name("Swing!!!")
                .Time(swingTime)
                .OnUpdate(data =>
                {
                    text.text = "Swing! : " + data.Progress.ToString("0.0");
                    Lerp(prepare, swing, Ease.InOutSine(data.Progress));
                })
                .Next(this)
                .Name("Cooldown...")
                .Time(cooldownTime)
                .OnUpdate(data =>
                {
                    text.text = "Cooldown... : " + data.Progress.ToString("0.0");
                    Lerp(swing, rest, Ease.InOutSine(data.Progress));
                })
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

        void Lerp(Transform a, Transform b, float t)
        {
            target.position = Vector3.Lerp(a.position, b.position, t);
            target.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
        }

    }
}
