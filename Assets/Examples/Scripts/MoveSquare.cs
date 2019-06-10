using UnityEngine;

namespace Momentum.Tests
{
    public class MoveSquare : MonoTaskable
    {
        void Start()
        {
            // TODO: make it loop

            new Sequence(this.GetTaskable())
                .AppendDelay(0.25f)
                .Append(new Task()
                    .Name("Up")
                    .Duration(1f)
                    .OnUpdate(_ => this.transform.position += Vector3.up * Time.deltaTime))
                .AppendDelay(0.25f)
                .Append(new Task()
                    .Name("Left")
                    .Duration(1f)
                    .OnUpdate(_ => this.transform.position += Vector3.left * Time.deltaTime))
                .AppendDelay(0.25f)
                .Append(new Task()
                    .Name("Down")
                    .Duration(1f)
                    .OnUpdate(_ => this.transform.position += Vector3.down * Time.deltaTime))
                .AppendDelay(0.25f)
                .Append(new Task()
                    .Name("Right")
                    .Duration(1f)
                    .OnUpdate(_ => this.transform.position += Vector3.right * Time.deltaTime))
                .Start();
        }
    }
}