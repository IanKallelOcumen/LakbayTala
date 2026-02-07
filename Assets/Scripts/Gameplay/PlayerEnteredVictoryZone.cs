using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Triggered when the player enters a VictoryZone. Completes level only if required Tala collected.
    /// </summary>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            if (model == null || model.player == null) return;
            bool hasEnoughTala = model.currentTalaCollected >= model.requiredTalaToFinish;
            if (!hasEnoughTala)
            {
                if (victoryZone != null && victoryZone.onNotEnoughTala != null)
                    victoryZone.onNotEnoughTala.Invoke();
                return;
            }
            if (model.player.animator != null)
                model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
        }
    }
}