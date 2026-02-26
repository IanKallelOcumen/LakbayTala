using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        void OnEnable()
        {
            Instance = this;
        }

        void Awake()
        {
            BackgroundBootstrap.EnsureBackgroundInUse();
        }

        void Start()
        {
            // Always find player by tag and ensure SimpleCheckpoint exists (don't rely on model.player)
            var playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null)
            {
                if (playerGo.GetComponent<SimpleCheckpoint>() == null)
                    playerGo.AddComponent<SimpleCheckpoint>();
                if (model != null)
                    model.player = playerGo.GetComponent<PlayerController>();
            }
            if (model != null)
                model.currentTalaCollected = 0;
            // Traps: add DeathTrigger so when player enters trigger, player dies
            foreach (var go in GameObject.FindGameObjectsWithTag("Trap"))
            {
                if (go.GetComponent<Collider2D>() != null && go.GetComponent<DeathTrigger>() == null)
                    go.AddComponent<DeathTrigger>();
            }
            foreach (var go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (go.GetComponent<EnemyDamagePlayer>() == null)
                    go.AddComponent<EnemyDamagePlayer>();
            }
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }
    }
}