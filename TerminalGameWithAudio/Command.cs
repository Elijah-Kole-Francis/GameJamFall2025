using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalGameWithAudio
{
    public class Command
    {
        public string name;
        String[] additionalAliases;
        
        public Command(string commandName, String[] additionalAliases = null) 
        {
            name = commandName;
            this.additionalAliases = (additionalAliases != null) ? additionalAliases : this.additionalAliases;
        }

        public bool DoesMatch(string input)
        {
            input = input.ToLower();

            return input.ToLower() == name.ToLower() || additionalAliases.Contains(input);
        }
    }
}
