using UnityEngine;

namespace Momentum
{
    // TODO: this should be added automatically (lazy instantiation)

    /// <summary>
    /// Unity object that updates the <see cref="T:Momentum.Juggler"/>
    /// </summary>
    public class Simulator : MonoBehaviour
    {
        [SerializeField] Juggler juggler;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            juggler = new Juggler();
        }

        void Update()
        {
            juggler.Update(Time.deltaTime);
        }
    }
}