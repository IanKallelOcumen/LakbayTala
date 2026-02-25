using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Victory trigger. Level completes only when player has collected required Tala (see PlatformerModel).
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        [Tooltip("Optional: fired when player reaches exit but has not collected enough Tala.")]
        public UnityEvent onNotEnoughTala;

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;
            }
        }
    }
}