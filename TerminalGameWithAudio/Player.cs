using MohawkTerminalGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TerminalGameWithAudio
{
    public class Player : Entity
    {
        public int currentArmour;
        public int maxArmour;
        public float hitPercentageVariance;

        private System.Random random = new System.Random();
        public Player(string name, int maxHealth, int baseDamage, float hitPercentage, float hitPercentageVariance, int maxArmour)
            : base(name, maxHealth, baseDamage, hitPercentage, hitPercentageVariance)
        {
            this.maxArmour = maxArmour;
            currentArmour = 0;
            this.hitPercentageVariance = hitPercentageVariance;
        }
        public bool TryHit()
        {
            float hitChange = (float)random.NextDouble() * (hitPercentageVariance * 2) - hitPercentageVariance;
            float finalChance = hitPercentage - hitChange;
            float roll = (float)random.NextDouble() * 100f;
            return roll < finalChance;
        }
        public void playerDamage(int amount)
        {
            if (!isAlive) return;

            if (currentArmour > 0)
            {
                int damageToArmour = Math.Min(amount, currentArmour);
                currentArmour -= damageToArmour;
                amount -= damageToArmour;
            }

            if (amount > 0)
            {
                currentHealth = Math.Max(currentHealth - amount, 0);
            }

            if (currentHealth == 0)
            {
                Kill();
            }
        }
        public void playerHeal(int amount)
        {
            currentHealth = Math.Min(currentHealth + amount, maxHealth);
        }
        public void playerBlock(int amount)
        {
            currentArmour = Math.Min(currentArmour + amount, maxArmour);
        }
        public void playerAttack(Entity target)
        {
            if (!isAlive) return;
            if (TryHit())
            {
                target.Damage(baseDamage);
            }
            else
            {
                Terminal.WriteLine("you missed");
            }
        }
    }
}
