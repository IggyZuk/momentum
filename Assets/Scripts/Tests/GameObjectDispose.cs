using Momentum;
using UnityEngine;

public class GameObjectDispose : MonoTaskable
{
    void Awake()
    {
        Task.Run(this)
            .Name("GameObjectDispose")
            .Time(1f)
            .Loop()
            .OnRepeat(data =>
            {
                Debug.Log(10 - data.CurrentLoop);
                if (data.CurrentLoop == 10) Destroy(this);
            });
    }
}
