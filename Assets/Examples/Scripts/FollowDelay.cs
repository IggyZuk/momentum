using UnityEngine;

namespace Momentum
{
    // TIP: you can inherit from MonoTaskable and be able to call Task.Run(this)

    public class FollowDelay : MonoTaskable
    {
        public float delay = 1f;

        void Start()
        {
            Task.Run(this)
                .Name("FollowDelay[input]")
                .Loop()
                .OnUpdate(_ =>
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pos.z = 0f;

                        Task.Run(this)
                            .Name("FollowDelay[sample]")
                            .Time(delay)
                            .OnComplete(__ =>
                            {
                                Debug.Log(pos);
                                this.transform.position = pos;
                            });
                    }
                });
        }
    }
}