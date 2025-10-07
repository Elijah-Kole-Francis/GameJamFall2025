using System;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        // Place your variables here
        // PLAYER
        int playerCurrentHealth = 73;
        int playerMaxHealth = 100;
        float playerHitPercentage = 0.75f;

        // ENEMY
        int enemyCurrentHealth = 7;
        int enemyMaxHealth = 10;
        float enemyHitPercentage = 0.75f;

       
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
            string healthBar = HealthDisplayText(playerCurrentHealth, playerMaxHealth);

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

            Terminal.WriteLine($"\tHit %: {playerHitPercentage * 100}%\n", ConsoleColor.Green, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintEnemyText()
        {

            string enemyhealthBar = HealthDisplayText(enemyCurrentHealth, enemyMaxHealth);

            string[] parts = enemyhealthBar.Split('|');
            if (parts.Length == 2)
            {
                Terminal.Write("ENEMY:\n\tHealth:[", ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[0], ConsoleColor.Red, ConsoleColor.Black);
                Terminal.Write(parts[1], ConsoleColor.DarkMagenta, ConsoleColor.Black);
                Terminal.WriteLine("]", ConsoleColor.Red, ConsoleColor.Black);
            }

            else
            {
                Terminal.WriteLine("ENEMY:\n" +
                    $"\tHealth: {enemyhealthBar}");
            }

            Terminal.WriteLine($"\tHit %: {enemyHitPercentage * 100}%\n", ConsoleColor.Red, ConsoleColor.Black);
            Terminal.WriteLine("", ConsoleColor.Black, ConsoleColor.Black);
        }

        void PrintOptionsText()
        {
            Terminal.WriteLine("PLAYER OPTIONS:\n" +
                $"\tOption 1\n" +
                $"\tOption 2\n" +
                $"\tOption 3\n" +
                $"\tOption 4\n" +
                "\n", ConsoleColor.Yellow, ConsoleColor.Black);
        }

        private string HealthDisplayText(int health, int maxHealth)
        {
            float healthPercentage = (float)health / maxHealth;
            int totalBars = 10;
            int filledBars = (int)(healthPercentage * totalBars);
            int emptyBars = totalBars - filledBars;

            string filled = new string('-', filledBars - 1) + $"{(int)(healthPercentage * 100)}%";
            string empty = new string('-', emptyBars);

            return $"{filled}|{empty}";
        }

    }
}
