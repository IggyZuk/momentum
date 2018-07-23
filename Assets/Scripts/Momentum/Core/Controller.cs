using UnityEngine;

namespace Momentum
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] Juggler juggler;

        void Awake()
        {
            juggler = Core.Juggler;
        }

        void Update()
        {
            Core.Juggler.Update(Time.deltaTime);
        }
    }
}