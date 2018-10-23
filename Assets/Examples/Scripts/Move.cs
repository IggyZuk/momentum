using UnityEngine;

namespace Momentum.Tests
{
    public class Move : MonoBehaviour
    {
        public Vector3 pos1;
        public Vector3 pos2;

        void Start()
        {
            bool positiveDirection = true;
            Color prevColor = Color.white;
            Color nextColor = Color.red;

            Task.Run(this.GetTaskable())
                .Name("L/R")
                .Order(1000)
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
                    Task.Run(this.GetTaskable())
                         .Name("Wait/Scale")
                         .Time(1)
                         .Random(0.7f)
                         .OnComplete(_ =>
                         {
                             positiveDirection = true;
                             var oldScale = this.transform.localScale;
                             var newScale = this.transform.localScale * 1.1f;

                             Task.Run(this.GetTaskable())
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

                             Task.Run(this.GetTaskable())
                                .Name("Wait/Loop")
                                .Time(1f)
                                .OnComplete(__ => data.Task.Start());
                         });
                });
        }
    }
}