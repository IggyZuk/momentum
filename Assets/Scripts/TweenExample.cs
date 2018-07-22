using UnityEngine;
using Momentum;

public class TweenExample : MonoBehaviour
{
    Vector2 pos = new Vector2();
    Vector2 dir = Vector2.right;
    Color color = Color.white;

    bool isSolid = false;

    [SerializeField] Juggler jugglerDebug;

    void Awake()
    {
        jugglerDebug = Core.Juggler;

        Tween tween = new Tween()
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

        tween.OnComplete(() =>
        {
            Debug.Log("Restart");
            tween.Reset();

            Core.Juggler.Add(new Tween().Time(3f).OnComplete(() =>
            {
                color = new Color(Random.value, Random.value, Random.value);
                Debug.Log("Boom!");
            }));
        });

        Core.Juggler.Add(
            new Tween().Time(1f).OnStart(() => color = Color.red).Next(
            new Tween().Time(1f).OnStart(() => color = Color.green).Next(
            new Tween().Time(1f).OnStart(() => color = Color.yellow).Next(
            new Tween().Time(1f).OnStart(() => color = Color.magenta).OnComplete(() => color = Color.cyan)
                .Next(new Tween().Time(1f).OnComplete(() =>
                {
                    Core.Juggler.Add(new Tween()
                        .Time(0.5f)
                        .Random(0.5f)
                        .OnRepeat(l => isSolid = !isSolid)
                        .Loop(-1));

                    Core.Juggler.Add(tween);
                }))
        ))));

        Core.Juggler.Add(new Tween()
            .OnUpdate(_ =>
            {
                Time.timeScale = Input.GetMouseButton(0) ? 4f : 1f;
            })
            .Time(1000f));
    }

    void Update()
    {
        Core.Juggler.Update(Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        if (isSolid) Gizmos.DrawSphere(pos, 1f);
        else Gizmos.DrawWireSphere(pos, 1f);
    }
}


