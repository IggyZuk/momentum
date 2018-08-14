using UnityEngine;
using Momentum;

public class GameObjectDispose : MonoBehaviour
{
    TaskDisposables disposables = new TaskDisposables();

    void Awake()
    {
        Task.Run()
            .Name("GameObjectDispose")
            .Time(1f)
            .Loop()
            .Dispose(disposables)
            .OnRepeat(data =>
            {
                Debug.Log(10 - data.CurrentLoop);
                if (data.CurrentLoop == 10) Component.Destroy(this);
            });
    }

    void OnDestroy()
    {
        disposables.Dispose();
    }
}
