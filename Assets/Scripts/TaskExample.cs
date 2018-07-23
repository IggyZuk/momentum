using UnityEngine;
using Momentum;

public class TaskExample : MonoBehaviour
{
    Vector2 pos = new Vector2();
    Vector2 dir = Vector2.right;
    Color color = Color.white;

    bool isSolid = false;

    void Awake()
    {
        Task task = new Task()
            .Delay(1f)
            .Time(1f)
            .Loop(3)
            .OnStart(() =>
            {
                Debug.Log("Start");
                dir = Vector2.right;
            })
            .OnUpdate((t) =>
            {
                pos += dir * Time.deltaTime;
            })
            .OnRepeat((l) =>
            {
                Debug.Log("Repeat: " + l);
                if (l == 1) dir = Vector2.up;
                if (l == 2) dir = Vector2.left;
                if (l == 3) dir = Vector2.down;
            });

        task.OnComplete(() =>
        {
            Debug.Log("Restart");
            task.Reset();

            Core.Juggler.Add(new Task().Time(3f).OnComplete(() =>
            {
                color = new Color(Random.value, Random.value, Random.value);
                Debug.Log("Boom!");
            }));
        });

        Core.Juggler.Add(
            new Task().Time(1f).OnStart(() => color = Color.red).Next(
            new Task().Time(1f).OnStart(() => color = Color.green).Next(
            new Task().Time(1f).OnStart(() => color = Color.yellow).Next(
            new Task().Time(1f).OnStart(() => color = Color.magenta).OnComplete(() => color = Color.cyan)
                .Next(new Task().Time(1f).OnComplete(() =>
                {
                    Core.Juggler.Add(new Task()
                        .Time(0.5f)
                        .Random(0.5f)
                        .OnRepeat(l => isSolid = !isSolid)
                        .Loop(-1));

                    Core.Juggler.Add(task);
                }))
        ))));

        Core.Juggler.Add(new Task()
            .OnUpdate(_ =>
            {
                Time.timeScale = Input.GetMouseButton(0) ? 4f : 1f;
            })
            .Time(1000f));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        if (isSolid) Gizmos.DrawSphere(pos, 1f);
        else Gizmos.DrawWireSphere(pos, 1f);
    }
}


