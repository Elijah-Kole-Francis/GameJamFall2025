using System;
using TerminalGameWithAudio;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        // COMMANDS
        readonly Command commandYes = new Command("yes", new[] { "yes", "y" });
        readonly Command commandNo = new Command("no", new[] {"no", "n"});
        readonly Command commandAttack = new Command("attack", new[] { "attack", "atk" });
        readonly Command commandBlock = new Command("block", new[] { "block", "blk" });
        readonly Command commandHeal = new Command("heal", new[] { "heal" });

        Command[] currentCommands;
        
        // PLAYER
        Player player = new Player("PLAYER", 100, 20, 0.75f, 20);

        // ENEMY
        Entity[] enemies = {
            new Entity("ENEMY 1", 25, 10, 0.5f),   
            new Entity("ENEMY 2", 50, 20, 0.75f),   
            new Entity("ENEMY 3", 100, 30, 1f),
        };
        
        int currentEnemyIndex = 0;

       
        /// Run once before Execute begins
        public void Setup()
        {
            // Program configuration
            Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteLoop;
            Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;

            // Hide raylib console output
            Terminal.ForegroundColor = ConsoleColor.Black;
            Terminal.CursorVisible = false;
            Audio.Initialize();
            
            // Move curosr to overwrite previously drawn (black) text
            Terminal.SetCursorPosition(0, 0);
            Terminal.ResetColor();
            Terminal.CursorVisible = true;

            currentCommands = new[] { commandAttack, commandBlock, commandHeal };
        }

        // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
        //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
        //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
        //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
        //               Code must finish within the alloted time frame for this to work well.
        public void Execute()
        {
            PrintPlayerText();
            PrintEnemyText();
            PrintOptionsText();
            
            string input = Terminal.ReadAndClearLine();
            ParseInput(input);

            Terminal.Clear();
        }

        void ParseInput(string input)
        {

        }

        void PrintPlayerText()
        {
            string healthBar = HealthDisplayText(player.currentHealth, player.maxHealth);

            string[] parts = healthBar.Split('|');
            if (parts.Length == 2)
            {
                Terminal.Write("PLAYER:\n\tHealth: [", ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(parts[0], ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(parts[1], ConsoleColor.DarkRed, ConsoleColor.Black);     
                Terminal.WriteLine("]", ConsoleColor.Green, ConsoleColor.Black);
            }
            else
            {
                Terminal.WriteLine("PLAYER:\n" +
                    $"\tHealth: {healthBar}", ConsoleColor.Green, ConsoleColor.Black);
            }

            Terminal.WriteLine($"\tHit %: {player.hitPercentage * 100}%\n", ConsoleColor.Green, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintEnemyText()
        {
            Entity currentEnemy = enemies[currentEnemyIndex];
            string enemyhealthBar = HealthDisplayText(currentEnemy.currentHealth, currentEnemy.maxHealth);

            string[] parts = enemyhealthBar.Split('|');
            if (parts.Length == 2)
            {
                Terminal.Write("ENEMY:\n\tHealth: [", ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[0], ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[1], ConsoleColor.DarkMagenta, ConsoleColor.Black);
                Terminal.WriteLine("]", ConsoleColor.Red, ConsoleColor.Black);
            }

            else
            {
                Terminal.WriteLine("ENEMY:\n" +
                    $"\tHealth: {enemyhealthBar}");
            }

            Terminal.WriteLine($"\tHit %: {currentEnemy.hitPercentage * 100}%\n", ConsoleColor.Red, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintOptionsText()
        {
            string optionsText = "";
            foreach (Command command in currentCommands)
            {
                optionsText += "\t" + command.name.ToUpper() + "\n";
            }
            
            Terminal.WriteLine("PLAYER OPTIONS:\n" +
                optionsText +
                "\n", ConsoleColor.Yellow, ConsoleColor.Black);
        }

        private string HealthDisplayText(int health, int maxHealth)
        {
            float healthPercentage = (float)(health) / (float)maxHealth;
            int totalBars = 10;
            int filledBars = (int)(healthPercentage * totalBars);
            int emptyBars = totalBars - filledBars;

            string filled = new string('-', filledBars - 1) + $"{(int)(healthPercentage * 100)}%";
            string empty = new string('-', emptyBars);

            return $"{filled}|{empty}";
        }

    }
}
