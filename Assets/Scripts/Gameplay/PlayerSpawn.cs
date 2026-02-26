using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            if (model == null || model.player == null || model.spawnPoint == null || model.virtualCamera == null) return;
            var player = model.player;
            if (player.collider2d != null) player.collider2d.enabled = true;
            player.controlEnabled = false;
            if (player.audioSource != null && player.respawnAudio != null)
                player.audioSource.PlayOneShot(player.respawnAudio);
            player.Teleport(model.spawnPoint.position);
            player.jumpState = PlayerController.JumpState.Grounded;
            if (player.animator != null) player.animator.SetBool("dead", false);
            var simpleCheckpoint = player.GetComponent<SimpleCheckpoint>();
            if (simpleCheckpoint != null) simpleCheckpoint.SetAlive();
            model.virtualCamera.Follow = player.transform;
            model.virtualCamera.LookAt = player.transform;
            Simulation.Schedule<EnablePlayerInput>(2f);
        }
    }
}