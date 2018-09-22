using UnityEngine;

namespace Momentum
{
    public class MoveSquare : MonoTaskable
    {
        void Start()
        {
            Task up = new Task(this);
            Task lt = new Task(this);
            Task dw = new Task(this);
            Task rt = new Task(this);

            up
                .Name("Up[delay]")
                .Random(0.125f)
                .Time(0.25f)
                .Next(this)
                .Name("Up")
                .Time(1f)
                .OnUpdate(_ => this.transform.position += Vector3.up * Time.deltaTime)
                .Next(lt);

            lt
                .Name("Left[delay]")
                .Random(0.125f)
                .Time(0.25f)
                .Next(this)
                .Name("Left")
                .Time(1f)
                .OnUpdate(_ => this.transform.position += Vector3.left * Time.deltaTime)
                .Next(dw);

            dw
                .Name("Down[delay]")
                .Random(0.125f)
                .Time(0.25f)
                .Next(this)
                .Name("Down")
                .Time(1f)
                .OnUpdate(_ => this.transform.position += Vector3.down * Time.deltaTime)
                .Next(rt);

            rt
                .Name("Right[delay]")
                .Random(0.125f)
                .Time(0.25f)
                .Next(this)
                .Name("Right")
                .Time(1f)
                .OnUpdate(_ => this.transform.position += Vector3.right * Time.deltaTime)
                .Next(up);

            up.Start();
        }
    }
}