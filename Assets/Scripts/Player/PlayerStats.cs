using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {

        public int maxHealthStart = 50;

        private int _maxHealth;
        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                int change = value - _maxHealth;
                _maxHealth = value;

                if (change > 0) Health += change;
                else Health = Health;
            }
        }

        private int _health;

        public int Health
        {
            get => _health;
            set => _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    
    
        // TODO TESTING VARS, REMOVE
        public bool setMaxHealth;
        public bool setHealth;
        public int maxHealthToSet;
        public int healthToSet;
    
        void Start()
        {
            MaxHealth = maxHealthStart;
        }

        void Update()
        {
            if (setHealth)
            {
                Health = healthToSet;
                setHealth = false;
                healthToSet = 0;
            }

            if (setMaxHealth)
            {
                MaxHealth = maxHealthToSet;
                setMaxHealth = false;
                maxHealthToSet = 0;
            }
        }


        public int GetHealth()
        {
            return Health;
        }

        public int GetMaxHealth()
        {
            return MaxHealth;
        }
    }
}
