using UnityEngine;

namespace LakbayTala.Mechanics
{
    public class StatusEffectSystem : MonoBehaviour
    {
        private HealthSystem healthSystem;

        // Simple timers for effects
        private float poisonTimer = 0f;
        private float burnTimer = 0f;
        private float stunTimer = 0f;

        public bool IsStunned => stunTimer > 0;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
        }

        private void Update()
        {
            HandlePoison();
            HandleBurn();
            HandleStun();
        }

        public void ApplyPoison(float duration)
        {
            poisonTimer = Mathf.Max(poisonTimer, duration);
        }

        public void ApplyBurn(float duration)
        {
            burnTimer = Mathf.Max(burnTimer, duration);
        }

        public void ApplyStun(float duration)
        {
            stunTimer = Mathf.Max(stunTimer, duration);
        }

        private void HandlePoison()
        {
            if (poisonTimer > 0)
            {
                poisonTimer -= Time.deltaTime;
                if (healthSystem) healthSystem.TakeDamage(2f * Time.deltaTime); // 2 DPS
            }
        }

        private void HandleBurn()
        {
            if (burnTimer > 0)
            {
                burnTimer -= Time.deltaTime;
                if (healthSystem) healthSystem.TakeDamage(5f * Time.deltaTime); // 5 DPS
            }
        }

        private void HandleStun()
        {
            if (stunTimer > 0)
            {
                stunTimer -= Time.deltaTime;
                // Logic to disable movement component would go here
            }
        }
    }
}
