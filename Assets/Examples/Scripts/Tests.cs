using UnityEngine;

namespace Momentum.Tests
{
    public class Tests : MonoTaskable
    {
        void Start()
        {
            TestGravity();
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
                .Time(0.1f)
                .Loop()
                .OnRepeat(_ =>
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
                .Time(5f)
                .OnUpdate(data => Debug.Log("Runner 1: " + data.Progress))
                .OnComplete(data =>
                {
                    Debug.Log(!locker ? "Runner 1: wins!" : "Runner 1: lost!");
                    if (!locker) locker = true;
                });

            Task.Run(this)
                .Name("Runner 2")
                .Order(1)
                .Time(5f)
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
                .Time(1f)
                .Loop()
                .OnRepeat(data => Debug.Log("Running Taskable"));

            taskable.RemoveTask(t);

            Task.Run(taskable)
                .Name("Taskable[remove]")
                .Time(5f)
                .OnComplete(data => taskable.StopAllTasks());
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

        void TestCircleUpdate()
        {
            Task.Run(this)
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
            Task.Run(this)
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