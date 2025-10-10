using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TerminalGameWithAudio
{
    public class Entity
    {
        public string name;
        public int currentHealth;
        public int maxHealth;
        public int baseDamage;
        public float hitPercentage;
        public float hitPercentageVariance;
        public bool isAlive = true;

        private Random random = new Random();
        public Entity(string name, int maxHealth, int baseDamage, float hitPercentage, float hitPercentageVariance)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.baseDamage = baseDamage;
            this.hitPercentage = hitPercentage;
            this.hitPercentageVariance = hitPercentageVariance;

            currentHealth = maxHealth;
        }
        public bool TryHit()
        {
            float hitChange = (float)random.NextDouble() * (hitPercentageVariance * 2) - hitPercentageVariance;
            float finalChance = hitPercentage - hitChange;
            float roll = (float)random.NextDouble() * 100f;
            return roll < finalChance;
        }
        public void Damage(int amount)
        {
            if (!isAlive) return;

            currentHealth = Math.Max(currentHealth - amount, 0);

            if (currentHealth == 0)
                Kill();
        }
        public void Kill()
        {
            if (isAlive) isAlive = false;
        }
    }
}