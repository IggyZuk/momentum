using UnityEngine;

namespace Momentum.Tests
{
    public class Tests : MonoTaskable
    {
        void Start()
        {
            TestSequence2();
            //TestSequence1();
            //TestNext();
            //TestGravity();
            //TestOrderTwo();
            //TestOrder();
            //TestRemoveTaskFromTaskable();
            //TestStop();
            //TestTaskable();
            //TestNext();
            //TestSwitchPositionsFromList();
            //TestCircleUpdate();
            //TestCircleLoop();
            TestTurbo();
        }

        void TestSequence1()
        {
            new Sequence(this.GetTaskable())
                .AppendDelay(5f)
                .AppendCallback(() => Debug.Log("Hello!"))
                .Start();
        }

        void TestSequence2()
        {
            new Sequence(this.GetTaskable())
                .Append(new Task()
                    .Name("One")
                    .Duration(1f)
                    .OnComplete(s => Debug.Log("1: " + s.Duration)))
                .AppendDelay(1f)
                .AppendCallback(() => Debug.Log("Hello World!"))
                .AppendDelay(1.25f)
                .Append(new Task()
                    .Name("Two")
                    .Duration(2f)
                    .OnComplete(s => Debug.Log("2: " + s.Duration)))
                .Append(new Task()
                    .Name("Three")
                    .Duration(3f)
                    .OnComplete(s => Debug.Log("3: " + s.Duration)))
                .AppendCallback(() => Debug.Log("A"))
                .AppendCallback(() => Debug.Log("B"))
                .AppendCallback(() => Debug.Log("C"))
                .AppendDelay(3f)
                .AppendCallback(() => Debug.Log("D"))
                .AppendCallback(() => Debug.Log("E"))
                .AppendCallback(() => Debug.Log("F"))
                .Start();
        }

        void TestGravity()
        {
            float y = 0f;
            bool isMouseDown = false;

            Task.Run(this)
                .Name("Gravity[input]")
                .Loop()
                .OnUpdate(_ => isMouseDown = Input.GetMouseButton(0));

            Task.Run(this)
                .Name("Gravity[gravity]")
                .Duration(0.1f)
                .Loop()
                .OnLoop(_ =>
                {
                    y += (isMouseDown ? 1f : -1f) * Time.deltaTime;
                    y = Mathf.Clamp01(y);
                });

            Task.Run(this)
                .Name("Gravity[movement]")
                .Loop()
                .OnUpdate(_ => MoveY(y));
        }

        void MoveY(float unitY)
        {
            this.transform.position = Vector3.LerpUnclamped(Vector3.down, Vector3.up, unitY);
        }

        void TestOrderTwo()
        {
            Task t1 = new Task().Name("Task(1)[3]").Order(3).Loop();
            Task t2 = new Task().Name("Task(2)[2]").Order(2).Loop();
            Task t3 = new Task().Name("Task(3)[1]").Order(1).Loop();

            t1.Start();
            t2.Start();
            t3.Start();

            Task.Run().Name("Task(4)[2]").Order(2).Loop();
            Task.Run().Name("Task(5)[0]").Order(0).Loop();
            Task.Run().Name("Task(6)[5]").Order(5).Loop();

            Task t7 = new Task().Name("Task(7)[3]").Order(3).Loop();
            Task t8 = new Task().Name("Task(8)[2]").Order(2).Loop();
            Task t9 = new Task().Name("Task(9)[1]").Order(1).Loop();

            t9.Start();
            t8.Start();
            t7.Start();

            Task.Run().Name("Task(10)[0]").Loop();

            Task t11 = new Task().Name("Task(11)[0]").Loop();
            t11.Start();
        }

        void TestOrder()
        {
            bool locker = false;

            Task.Run(this)
                .Name("Runner 1")
                .Order(-1)
                .Duration(5f)
                .OnUpdate(data => Debug.Log("Runner 1: " + data.Progress))
                .OnComplete(data =>
                {
                    Debug.Log(!locker ? "Runner 1: wins!" : "Runner 1: lost!");
                    if (!locker) locker = true;
                });

            Task.Run(this)
                .Name("Runner 2")
                .Order(1)
                .Duration(5f)
                .OnUpdate(data => Debug.Log("Runner 2: " + data.Progress))
                .OnComplete(data =>
                {
                    Debug.Log(!locker ? "Runner 2: wins!" : "Runner 2: lost!");
                    if (!locker) locker = true;
                });
        }

        void TestRemoveTaskFromTaskable()
        {
            Taskable taskable = new Taskable();

            Task t = Task.Run(taskable)
                .Name("Taskable[log]")
                .Duration(1f)
                .Loop()
                .OnLoop(data => Debug.Log("Running Taskable"));

            taskable.RemoveTask(t);

            Task.Run(taskable)
                .Name("Taskable[remove]")
                .Duration(5f)
                .OnComplete(data => taskable.StopAllTasks());
        }

        void TestStop()
        {
            Task.Run()
                .Name("Stop[main]")
                .Duration(1f)
                .Loop()
                .OnLoop(data =>
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
                .Duration(1f)
                .Loop()
                .AddTo(taskable)
                .OnLoop(data =>
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
            Task.Run()
                .Name("Loop")
                .Loop()
                .OnLoop(_ =>
                {
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        var seq = new Sequence(this.GetTaskable())
                            .Append(new Task().Name("1").Duration(1f).Loop(4).OnLoop(s => Debug.Log("1: " + s.CurrentLoop)))
                            .AppendCallback(() => Debug.Log("First is done"))
                            .Append(new Task().Name("2").Duration(0.5f).Loop(8).OnLoop(s => Debug.Log("2: " + s.CurrentLoop)))
                            .AppendCallback(() => Debug.Log("Second is done"))
                            .Append(new Task().Name("3").Duration(0.25f).Loop(16).OnLoop(s => Debug.Log("3: " + s.CurrentLoop)))
                            .AppendCallback(() => Debug.Log("Third is done"))
                            .Duration(3f)
                            .Start();

                        Task.Run().Loop().OnUpdate(__ => Debug.Log(seq.IsActive()));
                    }
                });
        }

        void TestCircleUpdate()
        {
            Task.Run(this)
                .Name("Circle Movement [update]")
                .Duration(1f)
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
            Task.Run(this)
                .Name("Circle Movement [loop]")
                .Loop()
                .OnLoop(data =>
                {
                    this.transform.position = new Vector3(
                        Mathf.Cos((data.CurrentLoop * 0.125f) % 360),
                        Mathf.Sin((data.CurrentLoop * 0.125f) % 360)
                    );
                });
        }

        void TestSwitchPositionsFromList()
        {
            Vector3[] positions = new Vector3[3];
            positions[0] = new Vector3(-1f, -1f, 0f);
            positions[1] = new Vector3(0f, 0f, 0f);
            positions[2] = new Vector3(1f, 1f, 0f);

            int posIndex = 0;

            new Sequence(this.GetTaskable())
                .AppendDelay(1f)
                .Append(
                    new Task()
                        .Name("TestSwitchPositionsFromList[loop]")
                        .Duration(0.25f)
                        .Loop()
                        .OnLoop(_ =>
                        {
                            posIndex++;
                            posIndex %= positions.Length;

                            this.transform.position = positions[posIndex];
                        }))
                .Start();
        }

        void TestTurbo()
        {
            Task.Run(this)
                .Name("Turbo")
                .Order(-1000)
                .Loop()
                .OnUpdate(_ =>
                {
                    Time.timeScale = Input.GetKey(KeyCode.T) ? 5f : 1f;
                });
        }
    }
}