using Momentum;
using UnityEngine;

public class Test : MonoTaskable
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;

    void Awake()
    {
        //TestGameObjectDestroy();
        //TestStop();
        //TestTaskable();
        //TestNext();
        //TestMoveDelay();
        //TestSwitchPositionsFromList();
        //TestMoveSquare();
        //TestCircleUpdate();
        //TestCircleLoop();
        //TestAttack();
        TestMoveAndScale();
        TestTurbo();
    }

    void TestGameObjectDestroy()
    {
        this.gameObject.AddComponent<GameObjectDispose>();
    }

    void TestStop()
    {
        Task.Run()
            .Name("Stop[main]")
            .Time(1f)
            .Loop()
            .OnRepeat(data =>
            {
                Debug.Log(10 - data.CurrentLoop);
                if (data.CurrentLoop == 10) data.Task.Stop();
            });
    }

    void TestTaskable()
    {
        Taskable taskable = new Taskable();

        Task.Run()
            .Name("Disposable[main]")
            .Time(1f)
            .Loop()
            .AddTo(taskable)
            .OnRepeat(data =>
            {
                Debug.Log(10 - data.CurrentLoop);
                if (data.CurrentLoop == 10) taskable.StopAllTasks();
            });

        Task.Run().Name("Disposable[1]").Loop().AddTo(taskable);
        Task.Run().Name("Disposable[2]").Loop().AddTo(taskable);
        Task.Run().Name("Disposable[3]").Loop().AddTo(taskable);
        Task.Run().Name("Disposable[4]").Loop().AddTo(taskable);
        Task.Run().Name("Disposable[5]").Loop().AddTo(taskable);
    }

    void TestNext()
    {
        Task.Run().Name("Loop").Loop().OnRepeat(_ =>
        {
            Task t1 = new Task().Name("1").Time(2f).Loop(4).OnRepeat(data => Debug.Log("1: " + data.CurrentLoop));
            Task t2 = new Task().Name("2").Time(1f).Loop(8).OnRepeat(data => Debug.Log("2: " + data.CurrentLoop));
            Task t3 = new Task().Name("3").Time(0.5f).Loop(16).OnRepeat(data => Debug.Log("3: " + data.CurrentLoop));

            if (Input.GetKeyDown(KeyCode.X))
            {
                Task.Run()
                    .Name("Starting...")
                    .Time(3f)
                    .Next(t1)
                    .Next(t2)
                    .Next(t3);
            }
        });
    }

    void TestAttack()
    {
        bool isAttacking = false;

        Task attackTask = new Task(this);

        attackTask
            .Name("Prepare")
            .Time(1f)
            .OnStart(_ => isAttacking = true)
            .Next(this)
            .Name("Attack")
            .Time(0.5f)
            .Next(this)
            .Name("Cooldown")
            .Time(2f)
            .OnComplete(_ => isAttacking = false);

        Task.Run(this)
            .Name("TestAttack")
            .Loop()
            .OnUpdate(_ =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!isAttacking)
                    {
                        attackTask.Start();
                    }
                }
            });
    }

    void TestCircleUpdate()
    {
        Task.Run()
            .Name("Circle Movement [update]")
            .Time(1f)
            .Loop()
            .OnUpdate(data =>
            {
                this.transform.position = new Vector3(
                    Mathf.Cos(data.Progress * Mathf.PI * 2f),
                    Mathf.Sin(data.Progress * Mathf.PI * 2f)
                );
            });
    }

    void TestCircleLoop()
    {
        Task.Run()
            .Name("Circle Movement [loop]")
            .Loop()
            .OnRepeat(data =>
            {
                this.transform.position = new Vector3(
                    Mathf.Cos((data.CurrentLoop * 0.125f) % 360),
                    Mathf.Sin((data.CurrentLoop * 0.125f) % 360)
                );
            });
    }

    void TestMoveSquare()
    {
        var t1 = new Task();
        var t2 = new Task();
        var t3 = new Task();
        var t4 = new Task();

        t1
            .Name("Up[delay]")
            .Time(0.25f)
            .Next()
            .Name("Up")
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.up * Time.deltaTime)
            .Next(t2);

        t2
            .Name("Left[delay]")
            .Time(0.25f)
            .Next()
            .Name("Left")
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.left * Time.deltaTime)
            .Next(t3);

        t3
            .Name("Down[delay]")
            .Time(0.25f)
            .Next()
            .Name("Down")
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.down * Time.deltaTime)
            .Next(t4);

        t4
            .Name("Right[delay]")
            .Time(0.25f)
            .Next()
            .Name("Right")
            .Time(1f)
            .OnUpdate(_ => this.transform.position += Vector3.right * Time.deltaTime)
            .Next(t1);

        t1.Start();
    }

    void TestMoveDelay()
    {
        Task.Run()
            .Name("TestDelay[loop]")
            .Loop()
            .OnUpdate(t =>
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pos.z = 0f;

                    Task.Run()
                        .Name("TestDelay[movement]")
                        .Time(1f)
                        .OnComplete(_ =>
                        {
                            Debug.Log(pos);
                            this.transform.position = pos;
                        });
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

        Task task = Task.Run()
            .Name("TestSwitchPositionsFromList[delay]")
            .Time(1f)
            .Next()
            .Name("TestSwitchPositionsFromList[loop]")
            .Time(0.25f)
            .Loop()
            .OnRepeat(_ =>
            {
                posIndex++;
                posIndex %= positions.Length;

                this.transform.position = positions[posIndex];
            });
    }

    void TestMoveAndScale()
    {
        bool positiveDirection = true;
        Color prevColor = Color.white;
        Color nextColor = Color.red;

        Task.Run()
            .Name("L/R")
            .Time(0.5f)
            .Loop(3)
            .Random(0f)
            .OnUpdate(data =>
            {
                Vector3 p = Vector3.LerpUnclamped(
                    positiveDirection ? pos1 : pos2,
                    positiveDirection ? pos2 : pos1,
                    Ease.OutBack(data.Progress)
                );
                this.transform.position = p;

                Color c = Color.LerpUnclamped(
                    prevColor,
                    nextColor,
                    Ease.OutBack(data.Progress)
                );
                this.GetComponent<MeshRenderer>().material.color = c;
            })
            .OnRepeat(data =>
            {
                prevColor = nextColor;
                nextColor = new Color(Random.value, Random.value, Random.value);
                positiveDirection = !positiveDirection;

            })
            .OnComplete(data =>
            {
                Task.Run()
                    .Name("Wait/Scale")
                    .Time(1)
                    .Random(0.7f)
                    .OnComplete(_ =>
                    {
                        positiveDirection = true;
                        var oldScale = this.transform.localScale;
                        var newScale = this.transform.localScale * 1.1f;

                        Task.Run()
                           .Name("Smooth Scale")
                           .Time(0.5f)
                           .OnUpdate(__ =>
                           {
                               var s = Vector3.LerpUnclamped(
                                   oldScale,
                                   newScale,
                                   Ease.InOutBack(__.Progress)
                               );
                               this.transform.localScale = s;
                           });

                        Task.Run()
                           .Name("Wait/Loop")
                           .Time(1f)
                           .OnComplete(__ => data.Task.Start());
                    });
            });
    }

    void TestTurbo()
    {
        Task.Run()
            .Name("Turbo")
            .Loop()
            .OnUpdate(_ =>
            {
                Time.timeScale = Input.GetKey(KeyCode.T) ? 5f : 1f;
            });
    }
}
