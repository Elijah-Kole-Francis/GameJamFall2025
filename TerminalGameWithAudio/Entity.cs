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

        public Entity(string name, int maxHealth, int baseDamage, float hitPercentage) 
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.baseDamage = baseDamage;
            this.hitPercentage = hitPercentage;
            
            currentHealth = maxHealth;
        }


    }
}
