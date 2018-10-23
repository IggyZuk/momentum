using UnityEngine;
using UnityEngine.UI;

namespace Momentum.Tests
{
    public class Counter : MonoBehaviour
    {
        public float time = 1f;
        public Text text;

        void Start()
        {
            Task.Run(this.GetTaskable())
                .Name(string.Format("Counter[{0}]", time))
                .Time(time)
                .Loop()
                .OnStart(data => text.text = data.CurrentLoop.ToString())
                .OnUpdate(data => text.transform.localScale = Vector3.one * (Ease.InOutSine(data.Progress * 2f) + 1))
                .OnRepeat(data => text.text = data.CurrentLoop.ToString());
        }

        /*
         * What this code looks like without momentum:
         * 
         * int count = 0;
         * float currentTime = 0f;
         * float time = 1f;
         * Text text;
         * 
         * void Update()
         * {
         *     currentTime += Time.deltaTime;
         *     
         *     if (currentTime >= time)
         *     {
         *         currentTime -= time;
         *         count++;
         *     }
         * 
         *     float progress = currentTime / time;
         *     text.transform.localScale = Vector3.one * (Ease.InOutSine(progress * 2f) + 1));
         * }
         */
    }
}