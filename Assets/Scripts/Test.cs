using UnityEngine;
using Momentum;

public class Test : MonoBehaviour
{
    void Awake()
    {
        //TestMoveDelay();
        //TestSwitchPositionsFromList();
        //TestMoveSquare();
        TestTurbo();

        Task.Add()
            .Name("Check")
            .Time(1000);

        Task.Add()
            .Loop(-1)
            .OnRepeat(i =>
            {
                this.transform.position = new Vector3(Mathf.Cos(i * 0.25f), Mathf.Sin(i * 0.25f), 0f);
            });
    }

    void TestTurbo()
    {
        Task.Add()
            .Name("Turbo")
            .Loop(-1)
            .OnRepeat(i =>
            {
                Time.timeScale = Input.GetMouseButton(0) ? 4f : 1f;
            });
    }

    void TestMoveSquare()
    {
        var t1 = new Task();
        var t2 = new Task();
        var t3 = new Task();
        var t4 = new Task();

        t1
            .Name("Up")
            .Delay(0.25f)
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.up * Time.deltaTime)
            .Next(t2);

        t2
            .Name("Left")
            .Delay(0.25f)
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.left * Time.deltaTime)
            .Next(t3);

        t3
            .Name("Down")
            .Delay(0.25f)
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.down * Time.deltaTime)
            .Next(t4);

        t4
            .Name("Right")
            .Delay(0.25f)
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.right * Time.deltaTime)
            .Next(t1);

        Core.Juggler.Add(t1);
    }

    void TestMoveDelay()
    {
        Task.Add()
        .Loop(-1)
        .OnUpdate(t =>
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0f;

                Task.Add()
                    .Time(1f)
                    .OnComplete(() => this.transform.position = pos);
            }
        });
    }

    void TestSwitchPositionsFromList()
    {
        Vector3[] positions = new Vector3[3];
        positions[0] = new Vector3(-1f, -1f, 0f);
        positions[1] = new Vector3(0f, 0f, 0f);
        positions[2] = new Vector3(1f, 1f, 0f);

        int posIndex = 0;

        var task = Task.Add();

        task
            .Delay(1f)
            .Time(0.25f)
            .Loop(-1)
            .OnRepeat(_ =>
            {
                posIndex++;
                posIndex %= positions.Length;

                this.transform.position = positions[posIndex];
            });
    }
}
