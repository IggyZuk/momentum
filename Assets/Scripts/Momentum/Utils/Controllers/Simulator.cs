using UnityEngine;

namespace Momentum
{
    public class Simulator : MonoBehaviour
    {
        [SerializeField] Juggler juggler;

        void Awake()
        {
            juggler = new Juggler();
        }

        void Update()
        {
            juggler.Update(Time.deltaTime);
        }
    }
}