using System;
using TerminalGameWithAudio;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        // Place your variables here
        // PLAYER
        Player player = new Player("PLAYER", 100, 20, 0.75f, 20);

        // ENEMY
        Entity enemy = new Entity("ENEMY", 100, 20, 0.75f);



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
            Terminal.ReadAndClearLine();
            Terminal.Clear();

        }

        void PrintPlayerText()
        {
            Terminal.WriteLine("PLAYER:\n" +
                $"\tHealth: {HealthDisplayText(player.currentHealth, player.maxHealth)}\n" +
                $"\tArmour: {HealthDisplayText(player.currentArmour, player.maxArmour)}\n" +
                $"\tHit %: {player.hitPercentage * 100}%\n" +
                "\n");

        }

        void PrintEnemyText()
        {
            Terminal.WriteLine("ENEMY:\n" +
                $"\tHealth: {HealthDisplayText(enemy.currentHealth, enemy.maxHealth)}\n" +
                $"\tHit %: {enemy.hitPercentage * 100}%\n" +
                "\n");

        }

        void PrintOptionsText()
        {
            Terminal.WriteLine("PLAYER OPTIONS:\n" +
                $"\tOption 1\n" +
                $"\tOption 2\n" +
                $"\tOption 3\n" +
                $"\tOption 4\n" +
                "\n");
        }

        private string HealthDisplayText(int health, int maxHealth)
        {
            string returnText = "";
            float healthPercentage = (float)health / (float)maxHealth;
            int healthIndex = (int)(healthPercentage * 10);
            int dashesAdded = 0;

            while (dashesAdded <= 10)
            {
                returnText += dashesAdded == healthIndex ? $"{(int)(healthPercentage * 100)}%" : "-";
                dashesAdded++;
            }

            return $"[{returnText}]";
        }
        
    }
}
