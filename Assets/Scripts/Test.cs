﻿using UnityEngine;
using Momentum;

public class Test : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;

    void Awake()
    {
        //TestStop();
        TestDispose();
        //TestNext();
        //TestMoveDelay();
        //TestSwitchPositionsFromList();
        //TestMoveSquare();
        TestTurbo();
        //TestCircle();
        //TestAttack();
        //TestMoveAndScale();
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

    void TestDispose()
    {
        TaskDisposables disposables = new TaskDisposables();

        Task.Run()
            .Name("Disposable[main]")
            .Time(1f)
            .Loop()
            .Dispose(disposables)
            .OnRepeat(data =>
            {
                Debug.Log(10 - data.CurrentLoop);
                if (data.CurrentLoop == 10) disposables.Dispose();
            });

        Task.Run().Name("Disposable[1]").Loop().Dispose(disposables);
        Task.Run().Name("Disposable[2]").Loop().Dispose(disposables);
        Task.Run().Name("Disposable[3]").Loop().Dispose(disposables);
        Task.Run().Name("Disposable[4]").Loop().Dispose(disposables);
        Task.Run().Name("Disposable[5]").Loop().Dispose(disposables);
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
                           .OnComplete(__ => Core.Juggler.Add(data.Task));
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

        Task.Run().Loop().OnRepeat(_ =>
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
        Task.Run()
            .Name("Circle Movement")
            .Time(1f)
            .Loop()
            .OnRepeat(data =>
            {
                this.transform.position = new Vector3(Mathf.Cos(data.CurrentLoop * 0.5f), Mathf.Sin(data.CurrentLoop * 0.5f), 0f);
            });
    }

    void TestTurbo()
    {
        Task.Run()
            .Name("Turbo")
            .Loop()
            .OnRepeat(_ =>
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
        Task.Run()
        .Loop()
        .OnUpdate(t =>
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0f;

                Task.Run()
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

        Task task = Task.Run()
            .Delay(1f)
            .Time(0.25f)
            .Loop()
            .OnRepeat(_ =>
            {
                posIndex++;
                posIndex %= positions.Length;

                this.transform.position = positions[posIndex];
            });
    }
}
