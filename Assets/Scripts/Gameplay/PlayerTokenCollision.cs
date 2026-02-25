using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player collides with a token (Tala). Increments level Tala count.
    /// </summary>
    public class PlayerTokenCollision : Simulation.Event<PlayerTokenCollision>
    {
        public PlayerController player;
        public TokenInstance token;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            if (model != null)
                model.currentTalaCollected++;
            if (token != null && token.tokenCollectAudio != null)
                AudioSource.PlayClipAtPoint(token.tokenCollectAudio, token.transform.position);
        }
    }
}