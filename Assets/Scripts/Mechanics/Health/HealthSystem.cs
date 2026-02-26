using UnityEngine;
using UnityEngine.Events;

namespace LakbayTala.Mechanics
{
    public class HealthSystem : MonoBehaviour
    {
        [Header("Configuration")]
        public float maxHealth = 100f;
        public float currentHealth;
        public bool isInvulnerable = false;

        [Header("Events")]
        public UnityEvent<float> OnDamageTaken;
        public UnityEvent<float> OnHealed;
        public UnityEvent OnDeath;
        public UnityEvent<float> OnHealthChanged; // Param is normalized health (0-1)

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (isInvulnerable || currentHealth <= 0) return;

            // Apply difficulty multiplier if applicable
            if (LakbayTala.Core.DifficultyManager.Instance != null)
            {
                amount *= LakbayTala.Core.DifficultyManager.Instance.GetCurrentConfig().enemyDamageMultiplier;
            }

            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);

            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(currentHealth / maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (currentHealth <= 0) return; // Can't heal dead entities

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            OnHealed?.Invoke(amount);
            OnHealthChanged?.Invoke(currentHealth / maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();
            // Optional: Destroy object or play animation
            // Destroy(gameObject, 2f); 
        }
    }
}
