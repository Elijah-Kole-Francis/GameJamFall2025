using System;
using System.Collections.Generic;
using System.Linq;
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
        }

    }
}
