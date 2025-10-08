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
        String[] aliases;
        
        public Command(string commandName, String[] aliases) 
        {
            name = commandName;
            this.aliases = aliases;
        }

        public bool DoesMatch(string input)
        {
            input = input.ToLower();

            return aliases.Contains(input);
        }
    }
}
