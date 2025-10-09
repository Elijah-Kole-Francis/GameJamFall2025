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
        readonly Command commandFireBall = new Command("fireball", new[] { "fireball", "fire" });
        readonly Command commandBlock = new Command("block", new[] { "block", "blk" });
        readonly Command commandHeal = new Command("heal", new[] { "heal" });

        Command commandEnemeyAttack = new Command("enemyattack", new[] {"enemy attack"});
        Command commandEnemyHeal = new Command("enemyheal", new[] { "enemy heal" });
        Command[] currentCommands;
        Command chosenCommand = null;
        Command enemyCommand = null;

        // PLAYER
        Player player = new Player("PLAYER", 100, 20, 0.75f, 100);

        // ENEMY
        Entity[] enemies = {
            new Entity("ENEMY 1", 100, 10, 0.5f),   
            new Entity("ENEMY 2", 100, 20, 0.75f),   
            new Entity("ENEMY 3", 100, 30, 1f),
        };
        
        int currentEnemyIndex = 0;

        System.Random random = new System.Random();

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

            currentCommands = new[] { commandAttack, commandFireBall, commandBlock, commandHeal};
            
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
            
            ParseInput();

            Terminal.Clear();
        }

        void ParseInput()
        {
            chosenCommand = null;
            
            while (true)
            {
                Terminal.Write("Please enter a command: ");

                string input = Terminal.ReadAndClearLine();
                int commandIndex = 0;

                foreach (Command command in currentCommands)
                {
                    if (command.DoesMatch(input))
                    {
                        chosenCommand = command;
                        break;
                    }
                }

                if (int.TryParse(input, out commandIndex) && chosenCommand == null)
                {
                    chosenCommand = currentCommands[commandIndex - 1];
                }

                if (chosenCommand != null) break;

                Terminal.ClearLine();
                Terminal.Write("Not a valid command. ");
            }

            ChooseEnemyCommand(); // can be moved after calling player command if need be - JM

            // Wanted to do switch statement but wasnt working and dont have time to figure ts out - JM
            if (chosenCommand == commandAttack) Attack();
            else if (chosenCommand == commandFireBall) FireBall();
            else if (chosenCommand == commandBlock) Block();
            else if (chosenCommand == commandHeal) Heal();

        }

        void Attack()
        {
            
            Terminal.WriteLine("Played Attack"); // For debug, can be removed - JM
            Entity enemy = enemies[currentEnemyIndex];
            int attackValue = random.Next(15, 26);
            enemy.Damage(attackValue);

        }

        void FireBall()
        {
            Entity enemy = enemies[currentEnemyIndex];
            int attackValue = random.Next(30, 40);
            int selfdamage = random.Next(10, 15);
            enemy.Damage(attackValue);
            player.playerDamage(selfdamage);
        }
        void Block()
        {
            Terminal.WriteLine("Played Blocked"); // For debug, can be removed - JM
            int blockValue = random.Next(10, 25);
            player.playerBlock(blockValue);
        }

        void Heal()
        {
            Terminal.WriteLine("Played Heal"); // For debug, can be removed - JM
            int healValue = random.Next(10, 15);
            player.playerHeal(healValue);
        }

        void ChooseEnemyCommand()
        {
            Command[] enemyAllowedCommands = {};
            enemyAllowedCommands = new[] { commandEnemeyAttack, commandEnemyHeal };
            int enemyIndex = random.Next(0, enemyAllowedCommands.Length);

            Command enemyIntention = enemyAllowedCommands[enemyIndex];

            if (enemyIntention == commandEnemeyAttack) enemyAttack();
            if (enemyIntention == commandEnemyHeal) enemyHeal();
            // For now just need it to randomly need to pick from allowed moves and apply to player - JM
        }
        void enemyAttack()
        {
            int attackValue = random.Next(15, 25);
            player.playerDamage(attackValue);
        }
        void enemyHeal()
        {
            Entity enemy = enemies[currentEnemyIndex];
            int healValue = random.Next(-15, -10);
            enemy.Damage(healValue);
        }
        void PrintPlayerText()
        {
            string healthBar = HealthDisplayText(player.currentHealth, player.maxHealth);

            string[] healthParts = healthBar.Split('|');
            if (healthParts.Length == 2)
            {
                Terminal.Write("PLAYER:\n\tHealth: [", ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(healthParts[0], ConsoleColor.Green, ConsoleColor.Black);
                Terminal.Write(healthParts[1], ConsoleColor.DarkRed, ConsoleColor.Black);     
                Terminal.WriteLine("]", ConsoleColor.Green, ConsoleColor.Black);
            }

            string armourBar = HealthDisplayText(player.currentArmour, player.maxArmour);
            string[] armourParts = armourBar.Split('|');
            if (armourParts.Length == 2)
            {
                Terminal.Write("\tArmor:  [", ConsoleColor.Gray, ConsoleColor.Black);
                Terminal.Write(armourParts[0], ConsoleColor.Gray, ConsoleColor.Black);
                Terminal.Write(armourParts[1], ConsoleColor.Black, ConsoleColor.Black);
                Terminal.WriteLine("]", ConsoleColor.Gray, ConsoleColor.Black);
            }
            else
            {
                Terminal.WriteLine("PLAYER:\n" +
                    $"\tHealth: {healthBar}", ConsoleColor.Green, ConsoleColor.Black);
            }

            Terminal.WriteLine($"\tHit %:  {player.hitPercentage * 100}%\n", ConsoleColor.Green, ConsoleColor.Black);
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
            float healthPercentage = (float)health / maxHealth;
            if (healthPercentage < 0f) healthPercentage = 0f;
            if (healthPercentage > 1f) healthPercentage = 1f;

            int totalBars = 10;
            int filledBars = (int)(healthPercentage * totalBars);
            int emptyBars = totalBars - filledBars;

            string filled;

            if (filledBars > 0)
            {
                filled = new string('-', filledBars - 1) + $"{(int)(healthPercentage * 100)}%";
            }
            else
            {
                filled = $"{(int)(healthPercentage * 100)}% ";
            }
            
            
            string empty = new string('-', emptyBars);
            return $"{filled}|{empty}";
            
            
        }

    }
}

