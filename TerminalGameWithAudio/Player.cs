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

        public Player(string name, int maxHealth, int baseDamage, float hitPercentage, int maxArmour)
            : base(name, maxHealth, baseDamage, hitPercentage)
        {
            this.maxArmour = maxArmour;
            currentArmour = 0;
            currentHealth = 80; // OK, override the base class value here
        }

        public void playerHeal(int amount)
        {
            currentHealth = Math.Min(currentHealth + amount, maxHealth);
        }

        public void playerBlock(int amount)
        {
            currentArmour = Math.Min(currentArmour + amount, maxArmour);
        }

        public void playerDamage(int amount)
        {
            if (!isAlive) return;
            if (currentArmour == 0)
            {
                currentHealth = Math.Max(currentHealth - amount, 0); // use Max to avoid negative health
            }
            else
            {
                currentArmour = Math.Max(currentArmour - amount, 0); // also avoid negative armour
            }

            if (currentHealth == 0) Kill();
        }
    }
}
