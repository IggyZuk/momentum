using UnityEngine;
using Momentum;

public class Test : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;

    void Awake()
    {
        //TestMoveDelay();
        //TestSwitchPositionsFromList();
        //TestMoveSquare();
        TestTurbo();
        //TestCircle();
        //TestAttack();
        TestMoveAndScale();
    }

    void TestMoveAndScale()
    {
        bool positiveDirection = true;

        Task.Add()
            .Name("L/R")
            .Time(0.5f)
            .Loop(3)
            .Random(0.3f)
            .OnUpdate(task =>
            {
                Vector3 p = Vector3.LerpUnclamped(
                    pos1,
                    pos2,
                    Ease.InOutBack(positiveDirection ? task.progress : 1f - task.progress)
                );

                this.transform.position = p;
            })
            .OnRepeat(task =>
            {
                positiveDirection = !positiveDirection;

            })
            .OnComplete(task =>
            {
                Task.Add()
                    .Name("Boom: " + task.currentLoop)
                    .Time(1)
                    .OnComplete(_ =>
                    {
                        positiveDirection = true;
                        this.transform.localScale = this.transform.localScale * 1.1f;

                        Task.Add()
                           .Name("Scale")
                           .Time(1)
                           .OnComplete(__ => Core.Juggler.Add(task));
                    });
            });
    }

    void TestAttack()
    {
        bool isAttacking = false;

        var attack =
            new Task().Name("Prepare").Time(1f).OnStart(_ => isAttacking = true).Next(
            new Task().Name("Attack").Time(0.5f).Next(
            new Task().Name("Cooldown").Time(2f).OnComplete(_ => isAttacking = false)));

        Task.Add().Loop(-1).OnRepeat(_ =>
        {
            if (Input.GetMouseButton(0))
            {
                if (!isAttacking)
                {
                    Core.Juggler.Add(attack);
                }
            }
        });
    }

    void TestCircle()
    {
        Task.Add()
            .Name("Circle Movement")
            .Loop(-1)
            .OnRepeat(task =>
            {
                this.transform.position = new Vector3(Mathf.Cos(task.currentLoop * 0.25f), Mathf.Sin(task.currentLoop * 0.25f), 0f);
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

        Task.Add()
            .Name("Check")
            .Time(1000);
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
                    .OnComplete(_ => this.transform.position = pos);
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
